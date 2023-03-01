import React from "react";
import ContentCli from "../../sdk/ContentCli";

import { ContentList } from "../contents/sections/ContentList";

const CONTENTS_PAGE_SIZE = 10

export class SekeerLayout extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      currentSearch: "",

      contents: [],

      endOfContents: null,

      pagesLoaded: 0,

      contentErrorMessage: null
    }
  }

  componentDidMount() { }

  loadNextPage = () => {
    this.setState({ isLoading: true })
    
    ContentCli.SearchContents(CONTENTS_PAGE_SIZE, this.state.pagesLoaded, this.state.currentSearch).then(response => {
      if (response !== null && response.toString().startsWith("ERROR")) {
        this.setState({ contentErrorMessage: response.substring(response.indexOf(' - ')) })
      } else {
        console.log("contents", response)
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

  handleOnInputChange = (event) => {
    var name = event.target.name
    var value = event.target.value

    this.setState({ [name]: value })
  }

  doSearch = () => {
    this.setState({ pagesLoaded: 0 }, () => {
      ContentCli.SearchContents(CONTENTS_PAGE_SIZE, this.state.pagesLoaded, this.state.currentSearch).then(response => {
        if (response !== null && response.toString().startsWith("[ERROR")) {
          this.setState({ contentsErrorMessage: response.substring(session.indexOf(' ')) })
        } else {
          console.log(response)
          this.setState({ contents: response })
        }
      })
    })
  }

  render() {
    return (
      <React.Fragment>
        <div className="container">
          <div className="row">
            <div className="col-md-12">

              <div className="row mt-5">
                <div className="col-md-12 mx-auto">
                  <div className="input-group">
                    <input className="form-control border-end-0 border" type="search" id="sekeer" name="currentSearch" placeholder="Busqueda de contenido..." list="datalistContents" value={this.state.currentSearch} onChange={this.handleOnInputChange} style={{ height: '60px' }} />
                    <span className="input-group-append" style={{ height: '60px' }}>
                      <button className="btn btn-outline-secondary bg-white border-start-0 border ms-n5" type="button" style={{ height: '100%', borderRadius: '0px 5px 5px 0px' }} onClick={this.doSearch}>
                        <i className="bi bi-search"></i>
                      </button>
                    </span>
                  </div>
                </div>
              </div>
              <datalist id="datalistContents">
                {this.state.contents.length > 0 && this.state.contents.map(content => {
                  return (<option key={content.id} value={content.title} />)
                })}
              </datalist>

              <ContentList contents={this.state.contents} user={null} handleContent={null}></ContentList>

              <div className="row mt-3">
                <div className="col-xs-12 col-md-12">
                  <button type="button" className="btn btn-primary float-end" onClick={this.loadNextPage} disabled={this.state.endOfContents}>Cargar más</button>
                </div>
              </div>

            </div>
          </div>
        </div>
      </React.Fragment>
    )
  }
}