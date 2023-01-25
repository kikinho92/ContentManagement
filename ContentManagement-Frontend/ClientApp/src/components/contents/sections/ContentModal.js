import React from 'react'
import ContentCli from '../../../sdk/ContentCli'

const initialState = {
  content: {
    id: null,
    title: "",
    description: "",
    link: "",
    authors: [],
    licenseTypes: [],
    tags: [],
    userid: "",
    uploadDate: null
  },
  user: null,

  currentAuthor: "",
  currentLicense: "",
  currentTag: ""
}

export class ContentModal extends React.Component {
  constructor(props) {
    super(props)
    this.state = initialState
  }

  componentDidMount() {
    const { content, user } = this.props

    if (content) { this.setState({ content: content }) }
    if (user) {
      this.setState({ user: user },
        () => {
          this.setState(prevState => ({
            content: {
              ...prevState.content,
              userid: this.state.user.id
            }
          }))
        })
    }
  }

  componentDidUpdate(prevProps) {
    if (prevProps.content !== this.props.content) {
      this.setState({ content: this.props.content })
    }
    else if (prevProps.user !== this.props.user) {
      this.setState({ user: this.props.user },
        () => {
          this.setState(prevState => ({
            content: {
              ...prevState.content,
              userid: this.state.user.id
            }
          }))
        })
    } else {
      return
    }
  }

  handleOnInputChange = (event) => {
    var name = event.target.name
    var value = event.target.value

    if (name !== "currentAuthor" && name !== "currentLicense" && name !== "currentTag") {
      this.setState(prevState => ({
        content: {
          ...prevState.content,
          [name]: value
        }
      }))
    } else {
      this.setState({ [name]: value })
    }
  }

  addItemList = (event) => {
    var name = event.target.name

    switch (name.toUpperCase()) {
      case "AUTHOR":
        this.setState(prevState => ({
          content: {
            ...prevState.content,
            authors: [this.state.currentAuthor, ...this.state.content.authors]
          }
        }), () => this.setState({ currentAuthor: "" }))
        break;
      case "LICENSE":
        this.setState(prevState => ({
          content: {
            ...prevState.content,
            licenseTypes: [...this.state.content.licenseTypes, this.state.currentLicense]
          }
        }), () => this.setState({ currentLicense: "" }))
        break;
      case "TAG":
        this.setState(prevState => ({
          content: {
            ...prevState.content,
            tags: [...this.state.content.tags, { id: "", name: this.state.currentTag, userid: this.state.user.id, uploadDate: null }]
          }
        }), () => this.setState({ currentTag: "" }))
        break;
    
      default:
        break;
    }

    
  }

  removeItemList = (event) => {
    var name = event.target.name

    switch (name.toUpperCase()) {
      case "AUTHOR":
        const authors = this.state.authors.filter(author => author !== this.state.currentAuthor)
        this.setState(prevState => ({
          content: {
            ...prevState.content,
            authors: authors
          }
        }))
        break;
      case "LICENSE":
        const licenses = this.state.licenseTypes.filter(license => license !== this.state.currentLicense)
        this.setState(prevState => ({
          content: {
            ...prevState.content,
            licenseTypes: licenses
          }
        }))
        break;
      case "TAG":
        const tags = this.state.tags.filter(tag => tag.name !== this.state.currentTag)
        this.setState(prevState => ({
          content: {
            ...prevState.content,
            tags: tags
          }
        }))
        break;
    
      default:
        break;
    }
  }

  handleSaveContent = () => {

    const { handleContent } = this.props

    if (this.state.content.id === null) {
      ContentCli.PostContent(this.state.content).then(response => {
        if (response !== null && response.toString().startsWith("ERROR")) {
          this.setState({ contentErrorMessage: response.substring(response.indexOf(' - ')) })
        } else {
          this.setState(initialState, () => {
            this.setState({ contentSuccessMessage: "¡Contenido guardado correctamente!" })
            handleContent(response, "POST")
          })
        }
      })
    } else {
      ContentCli.PutContent(this.state.content, this.state.content.id).then(response => {
        if (response !== null && response.toString().startsWith("ERROR")) {
          this.setState({ contentErrorMessage: response.substring(response.indexOf(' - ')) })
        } else {
          this.setState(initialState, () => {
            this.setState({ contentSuccessMessage: "¡Contenido modificado correctamente!" })
            handleContent(response, "PUT")
          })
        }
      })
    }
  }

  render() {
    return (
      <React.Fragment>

        <div className="modal fade" id={"content-modal-" + (this.state.content ? this.state.content.id : "")} tabIndex={-1} aria-labelledby="content-modal-label" aria-hidden="true">
          <div className="modal-dialog modal-lg">
            <div className="modal-content">
              <div className="modal-header">
                <h1 className="modal-title fs-5" id="content-modal-label">Contenido</h1>
                <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
              </div>
              <div className="modal-body">
                <div className="container-fluid">
                  <div className='row p-3'>
                    <div className='col-xs-12 col-md-4'>
                      <input className="form-control" type="text" placeholder="Título" name="title" aria-label="input-title" value={this.state.content ? this.state.content.title : ""} onChange={this.handleOnInputChange} />
                    </div>
                    <div className='col-xs-12 col-md-8'>
                      <input className="form-control" type="text" placeholder="Link" name="link" aria-label="input-link" value={this.state.content ? this.state.content.link : ""} onChange={this.handleOnInputChange} />
                    </div>
                  </div>
                  <div className='row p-3'>
                    <div className='col-xs-12 col-md-12'>
                      <textarea className="form-control" id="input-description" name="description" rows="3" value={this.state.content ? this.state.content.description : ""} onChange={this.handleOnInputChange}></textarea>
                    </div>
                  </div>
                  <div className='row p-3'>
                    <div className='col-xs-12 col-md-4'>
                      <div className="input-group mb-3">
                        <input className="form-control" type="text" placeholder="Autor" name="currentAuthor" aria-label="input-author" aria-describedby="button-author" value={this.state.currentAuthor} onChange={this.handleOnInputChange} />
                        <button className="btn btn-primary" type="button" id="button-author" name="author" onClick={this.addItemList}>Añadir</button>
                      </div>
                      <ul className="list-group">
                        {this.state.content && this.state.content.authors && this.state.content.authors.map(author => {
                          return (
                            <li key={author} className="list-group-item">
                              {author}
                              <i className="bi bi-trash-fill float-end" name="author" style={{ cursor: 'pointer' }} onClick={this.removeItemList}></i>
                            </li>
                          )
                        })}
                      </ul>
                    </div>
                    <div className='col-xs-12 col-md-4'>
                      <div className="input-group mb-3">
                        <input className="form-control" type="text" placeholder="Tipos de licencia" name="currentLicense" aria-label="input-license" value={this.state.currentLicense} onChange={this.handleOnInputChange} />
                        <button className="btn btn-primary" type="button" id="button-license" name="license" onClick={this.addItemList}>Añadir</button>
                      </div>
                      <ul className="list-group">
                        {this.state.content && this.state.content.licenseTypes && this.state.content.licenseTypes.map(license => {
                          return (
                            <li key={license} className="list-group-item">
                              {license}
                              <i className="bi bi-trash-fill float-end" name="license" style={{ cursor: 'pointer' }} onClick={this.removeItemList}></i>
                            </li>
                          )
                        })}
                      </ul>
                    </div>
                    <div className='col-xs-12 col-md-4'>
                      <div className="input-group mb-3">
                        <input className="form-control" type="text" placeholder="Etiquetas" name="currentTag" aria-label="input-tag" value={this.state.currentTag} onChange={this.handleOnInputChange} />
                        <button className="btn btn-primary" type="button" id="button-tag" name="tag" onClick={this.addItemList}>Añadir</button>
                      </div>
                      <ul className="list-group">
                        {this.state.content && this.state.content.tags && this.state.content.tags.map(tag => {
                          return (
                            <li key={tag.name} className="list-group-item">
                              {tag.name}
                              <i className="bi bi-trash-fill float-end" name="tag" style={{ cursor: 'pointer' }} onClick={this.removeItemList}></i>
                            </li>
                          )
                        })}
                      </ul>
                    </div>
                  </div>
                </div>
              </div> 
              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                <button type="button" className="btn btn-primary" onClick={this.handleSaveContent} data-bs-dismiss="modal">Guardar</button>
              </div>
            </div>
          </div>
        </div>
        
        {this.state.contentErrorMessage != null &&
          <div className="alert alert-primary" role="alert">
            {this.state.contentErrorMessage}
          </div>
        }
        {this.state.contentSuccessMessage != null &&
          <div className="alert alert-success" role="alert">
            {this.state.contentSuccessMessage}
          </div>
        }

      </React.Fragment>
    )
  }
}