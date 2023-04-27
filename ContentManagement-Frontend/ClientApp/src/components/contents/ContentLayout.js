import React from 'react'
import AuthCli from '../../sdk/AuthCli'
import ContentCli from '../../sdk/ContentCli'
import UserCli from '../../sdk/UserCli'

import { ContentModal } from './sections/ContentModal'
import { ContentList } from './sections/ContentList'
import { ContentModalExcelUpload } from './sections/ContentModalExcelUpload'

const CONTENTS_PAGE_SIZE = 10

export class ContentLayout extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      isLoading: false,

      user: null,

      contents: [],

      endOfContents: false,

      pagesLoaded: 0,

      contentErrorMessage: null
    }
  }

  componentDidMount() {

    AuthCli.GetSessionInfoAsync().then(session => {
      if (!session) {
        this.setState({ contentErrorMessage: session.substring(session.indexOf(' ')) })
      } else {
        console.log("session", session)
        UserCli.GetUser(session.userId).then(response => {
          console.log("user", response)
          if (response !== null && response.toString().startsWith("ERROR")) {
            this.setState({ contentErrorMessage: response.substring(response.indexOf(' - ')) })
          } else {
            this.setState({ user: response }, () => { this.loadNextPage() })
            
          }
        })
      }
    })
  }

  loadNextPage = () => {
    this.setState({ isLoading: true })
    
    ContentCli.GetContents(CONTENTS_PAGE_SIZE, this.state.pagesLoaded, null, this.state.user.group.id).then(response => {
      if (response !== null && response.toString().startsWith("ERROR")) {
        this.setState({ contentErrorMessage: response.substring(response.indexOf(' - ')) })
      } else {
        if (response.length > 0) {
          this.setState(previousState => ({
            contents: [...previousState.contents, ...response],
            pagesLoaded: this.state.pagesLoaded + 1,
            contentErrorMessage: null
          }))
        } else {
          this.setState({ contents: response })
        }

        if (response.length < CONTENTS_PAGE_SIZE) {
          this.setState({ endOfContents: true, contentErrorMessage: null })
        }
      }
    })
  }

  handleContent = (content, option) => {

    if (option === "POST") {
      this.setState(previousState => ({
        contents: [...previousState.contents, content]
      }))
    } else {
      const contents = this.state.contents.map(c => {
        if (c.id === content.id) {
          return content
        } else {
          return c
        }
      })
      this.setState({ contents: contents })
    }
  }

  handleMultipleContents = (contents) => {
    contents.forEach(content => {
      this.setState({ contents: [...this.state.contents, content]})
    });
  }

  render() {
    return (
      <React.Fragment>
        <div className="row">
          <div className="col-xs-12 col-md-8">
            <h2>{this.state.user ? this.state.user.group.name : ""}</h2>
          </div>
          <div className="col-xs-12 col-md-2">
            <button type="button" className="btn btn-success custom-btn-success" data-bs-toggle="modal" data-bs-target="#content-modal-excel" style={{ width: "100%" }}>
              Carga de contenidos
              <i className="bi bi-file-earmark-spreadsheet-fill" style={{marginLeft: "5px"}}></i>
            </button>
          </div>
          <div className="col-xs-12 col-md-2">
            <button type="button" className="btn btn-primary custom-btn-primary" data-bs-toggle="modal" data-bs-target="#content-modal-null" style={{width: "100%"}}>Añadir contenido</button>
          </div>
        </div>

        <ContentList contents={this.state.contents} user={this.state.user} handleContent={this.handleContent}></ContentList>

        <div className="row mt-3">
          <div className="col-xs-12 col-md-12">
            <button type="button" className="btn btn-primary float-end custom-btn-primary" onClick={this.loadNextPage} disabled={this.state.endOfContents}>Cargar más</button>
          </div>
        </div>
        
        <ContentModal content={null} user={this.state.user} handleContent={this.handleContent}></ContentModal>

        <ContentModalExcelUpload handleMultipleContents={this.handleMultipleContents} user={this.state.user}></ContentModalExcelUpload>
      </React.Fragment>
    )
  }
}