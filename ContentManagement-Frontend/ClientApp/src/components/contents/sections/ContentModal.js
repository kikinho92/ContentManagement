import React from 'react'
import ContentCli from '../../../sdk/ContentCli'

const initialState = {
  content: {
    id: null,
    title: "",
    description: "",
    link: "",
    authors: [],
    grades: [],
    tags: [],
    licenseTypes: [],
    userid: "",
    uploadDate: null
  },
  user: null,
  tags: [],

  currentAuthor: "",
  currentGrade: "",
  currentTag: "",

  response: null
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

          ContentCli.GetTags(this.state.user.group.id).then(response => {
            if (response !== null && response.toString().startsWith("[ERROR")) {
              this.setState({ tagsErrorMessage: response.substring(session.indexOf(' ')) })
            } else {
              this.setState({ tags: response })
            }
          });
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

    if (name !== "currentAuthor" && name !== "currentGrade" && name !== "currentTag") {
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
    if (event.target.tagName.toUpperCase() == "I") {
      name = event.target.parentElement.name
    }

    switch (name.toUpperCase()) {
      case "AUTHOR":
        this.setState(prevState => ({
          content: {
            ...prevState.content,
            authors: [this.state.currentAuthor, ...this.state.content.authors]
          }
        }), () => this.setState({ currentAuthor: "" }))
        break;
      case "GRADE":
        this.setState(prevState => ({
          content: {
            ...prevState.content,
            grades: [...this.state.content.grades, this.state.currentGrade]
          }
        }), () => this.setState({ currentGrade: "" }))
        break;
      case "TAG":
        var tag = this.state.tags.find(t => t.name === this.state.currentTag)
        if (!tag) {
          tag = { id: "", name: this.state.currentTag, userid: this.state.user.id, uploadDate: null }
        }
        this.setState(prevState => ({
          content: {
            ...prevState.content,
            tags: [...this.state.content.tags, tag]
          }
        }), () => this.setState({ currentTag: "" }))
        break;
    
      default:
        break;
    }
  }

  removeItemList = (event) => {
    var name = event.target.dataset.name
    var value = event.target.parentElement.textContent

    switch (name.toUpperCase()) {
      case "AUTHOR":
        const authors = this.state.content.authors.filter(author => author !== value)
        this.setState(prevState => ({
          content: {
            ...prevState.content,
            authors: authors
          }
        }))
        break;
      case "GRADE":
        const grades = this.state.content.grades.filter(grade => grade !== value)
        this.setState(prevState => ({
          content: {
            ...prevState.content,
            grades: grades
          }
        }))
        break;
      case "TAG":
        const tags = this.state.content.tags.filter(tag => tag.name !== value)
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

    if (this.state.content.id === null) {
      ContentCli.PostContent(this.state.content).then(response => {
        if (response !== null && response.toString().startsWith("ERROR")) {
          this.setState({ contentErrorMessage: response.substring(response.indexOf(' - ')) })
        } else {
          this.setState(initialState, () => {
            this.setState({ contentSuccessMessage: "¡Contenido guardado correctamente!" })
            this.props.handleContent(response, "POST")
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
            this.props.handleContent(response, "PUT")
          })
        }
      })
    }
  }

  render() {
    return (
      <React.Fragment>

        <div className="modal fade" id={"content-modal-" + (this.state.content ? this.state.content.id : "")} tabIndex={-1} aria-labelledby="content-modal-label" aria-hidden="true">
          <div className="modal-dialog modal-xl">
            <div className="modal-content custom-modal-content">
              <div className="modal-header">
                <h1 className="modal-title fs-5" id="content-modal-label">Gestión de contenidos</h1>
                <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
              </div>
              <div className="modal-body">
                <div className="container-fluid">
                  
                  <div className='row p-2'>
                    <div className='col-xs-12 col-md-12'>
                      <div className='row p-1'>
                        <div className='col-md-12'>
                          <h4>Información del contenidos</h4>
                        </div>
                      </div>
                      <div className='row p-1'>
                        <div className='col-xs-12 col-md-4'>
                          <label htmlFor="input-title" className="form-label">Título</label>
                          <input id="input-title" className="form-control custom-input" type="text" placeholder="Título" name="title" aria-label="input-title" value={this.state.content ? this.state.content.title : ""} onChange={this.handleOnInputChange} />
                        </div>
                        <div className='col-xs-12 col-md-8'>
                          <label htmlFor="input-link" className="form-label">Link</label>
                          <input id="input-link" className="form-control custom-input" type="text" placeholder="Link" name="link" aria-label="input-link" value={this.state.content ? this.state.content.link : ""} onChange={this.handleOnInputChange} />
                        </div>
                      </div>
                      <div className='row p-1'>
                        <div className='col-xs-12 col-md-12'>
                          <label htmlFor="input-description" className="form-label">Descripción</label>
                          <textarea id="input-description" className="form-control custom-input" placeholder="Descripción" name="description" rows="3" value={this.state.content ? this.state.content.description : ""} onChange={this.handleOnInputChange}></textarea>
                        </div>
                      </div>
                    </div>
                  </div>

                  <hr></hr>
                  
                  <div className='row p-1'>
                    <div className='col-xs-12 col-md-6'>
                      <div className='row p-1'>
                        <div className='col-md-12'>
                          <h4>Autor/es</h4>
                        </div>
                      </div>
                      <div className='row p-1'>
                        <div className='col-md-12'>
                          <div className="input-group mb-3">
                            <input className="form-control custom-input" type="text" placeholder="Autor" name="currentAuthor" aria-label="input-author" aria-describedby="button-author" value={this.state.currentAuthor} onChange={this.handleOnInputChange} />
                            <button className="btn btn-primary custom-btn-primary" type="button" id="button-author" name="author" onClick={this.addItemList}>
                              <i className="bi bi-plus-circle"></i>
                            </button>
                          </div>
                          <ul className="list-group">
                            {this.state.content && this.state.content.authors && this.state.content.authors.map(author => {
                              return (
                                <li key={author} className="list-group-item custom-list-group-item">
                                  {author}
                                  <i className="bi bi-trash-fill float-end" data-name="author" style={{ cursor: 'pointer' }} onClick={this.removeItemList}></i>
                                </li>
                              )
                            })}
                          </ul>
                        </div>
                      </div>
                    </div>
                    <div className='col-xs-12 col-md-6'>
                      <div className='row p-1'>
                        <div className='col-md-12'>
                          <h4>Titulación/es</h4>
                        </div>
                      </div>
                      <div className='row p-1'>
                        <div className='col-md-12'>
                          <div className="input-group mb-3">
                            <input className="form-control custom-input" type="text" placeholder="Titulación" name="currentGrade" aria-label="input-grades" value={this.state.currentGrade} onChange={this.handleOnInputChange} />
                            <button className="btn btn-primary custom-btn-primary" type="button" id="button-grade" name="grade" onClick={this.addItemList}>
                              <i className="bi bi-plus-circle"></i>
                            </button>
                          </div>
                          <ul className="list-group">
                            {this.state.content && this.state.content.grades && this.state.content.grades.map(grade => {
                              return (
                                <li key={grade} className="list-group-item custom-list-group-item">
                                  {grade}
                                  <i className="bi bi-trash-fill float-end" data-name="grade" style={{ cursor: 'pointer' }} onClick={this.removeItemList}></i>
                                </li>
                              )
                            })}
                          </ul>
                        </div>
                      </div>
                    </div>
                  </div>

                  <hr></hr>

                  <div className='row p-3'>
                    <div className='col-xs-12 col-md-12'>
                      <div className='row p-1'>
                        <div className='col-md-12'>
                          <h4>Etiquetas</h4>
                        </div>
                      </div>
                      <div className='row p-1'>
                        <div className='col-xs-12 col-md-6'>
                          <div className="input-group mb-3">
                            <input className="form-control custom-input" list="datalistTags" id={"datalist-" + this.state.content.id} name="currentTag" placeholder="Etiquetas..." value={this.state.currentTag} onChange={this.handleOnInputChange} />
                            <button className="btn btn-primary custom-btn-primary" type="button" id="button-tag" name="tag" onClick={this.addItemList}>
                              <i name="tag" className="bi bi-plus-circle"></i>
                            </button>
                            <datalist id="datalistTags">
                              {this.state.tags && this.state.tags.map(tag => {
                                return (<option key={tag.id} value={tag.name} />)
                              })}
                            </datalist>
                          </div>
                        </div>
                      </div>
                      <div className='row p-1'>
                        <div className='col-xs-12 col-md-12'>
                          {this.state.content && this.state.content.tags && this.state.content.tags.map(tag => {
                            return (
                              <span key={tag.name} className="badge rounded-pill text-bg-success mx-1">
                                {tag.name} 
                                <i className="bi bi-trash-fill float-end" data-name="tag" style={{ cursor: 'pointer', marginLeft: "10px" }} onClick={this.removeItemList}></i>
                              </span>
                            )
                          })}
                          
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-secondary custom-btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                <button type="button" className="btn btn-primary custom-btn-primary" onClick={this.handleSaveContent} data-bs-dismiss="modal">Guardar</button>
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