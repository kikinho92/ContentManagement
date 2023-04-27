import React from "react";

import { ContentModal } from "./ContentModal";

import UserCli from "../../../sdk/UserCli";

const URJC = "UNIVERSIDAD REY JUAN CARLOS (MÓSTOLES)"
const UC3M = "UNIVERSIDAD CARLOS III (LEGANÉS)"
const UAH = "UNIVERSIDAD DE ALCALÁ"

export class ContentSingle extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      userContentError: null,

      ownerLogo: ""
    }
  }

  componentDidMount() {
    this.getOwnerLogo()
  }

  getOwnerLogo = () => {
    if (this.props.content) {
      UserCli.GetUser(this.props.content.userid).then(response => {
        if (response !== null && response.toString().startsWith("[ERROR")) {
          this.setState({ userContentError: response.substring(response.indexOf(' ')) })
        } else {
          var user = response
          var logo = "";
          if (user && user.group) {
            switch (user.group.name.toUpperCase()) {
              case URJC:
                logo = "custom-urjc-image"
                break;
              case UC3M:
                logo = "custom-uc3m-image"
                break;
              case UAH:
                logo = "custom-uah-image"
                break;
              default:
                break;
            }
            this.setState({ ownerLogo: logo })
          }
        }
      })
    }
  }

  render() {

    const { content, user, handleContent } = this.props

    return (
      <React.Fragment>
        <div className={"card mt-3 custom-card " + this.state.ownerLogo}>
          <div className="row no-gutters">
            {/* <div className="col-xs-12 col-md-4">
              <img className="card-img img-thumbnail img-fluid rounded mx-auto d-block" src={UrjcLogoNoWords} width={150} alt="" style={{ width: "200px", position: "relative", top: "calc(50% - 100px)" }} />
            </div> */}
            <div className="col-xs-12 col-md-12">
              <div className="card-body">
                <h5 className="card-title">{content.title}
                  {this.props.user && 
                    <i className="bi bi-pencil-square float-end" data-bs-toggle="modal" data-bs-target={"#content-modal-" + content.id} style={{ cursor: "pointer" }}></i>
                  }
                </h5>
                <hr className="mt-1 mb-1" />
                <div className="row">
                  <div className="col-xs-12 col-md-12">
                    <span className="custom-card-title-text">Enlace</span>
                    <p className="card-text custom-card-text ms-3"><a href={content.link} target="_blank" rel="">Link del contenido</a></p>
                  </div>
                </div>
                {/* <hr className="mt-1 mb-1" /> */}
                {content.description && 
                  <div className="row">
                    <div className="col-xs-12 col-md-12">
                      <span className="custom-card-title-text">Descripción</span>
                      <p className="card-text custom-card-text ms-3">{content.description}</p>
                    </div>
                  </div>
                }
                {/* <hr className="mt-1 mb-1" /> */}
                <div className="row">
                  <div className="col-xs-12 col-md-12">
                    <span className="custom-card-title-text">Autor/es</span>
                    <p className="card-text custom-card-text ms-3">{content.authors ? content.authors.join("\r\n") : "-"}</p>
                  </div>
                </div>
                {/* <hr className="mt-1 mb-1" /> */}
                {content.department &&
                  <div className="row">
                    <div className="col-xs-12 col-md-12">
                      <span className="custom-card-title-text">Departamento</span>
                      <p className="card-text custom-card-text ms-3 custom-card-break-text">{content.department ? content.department : "-"}</p>
                    </div>
                  </div>
                }
                {/* <hr className="mt-1 mb-1" /> */}
                <div className="row">
                  <div className="col-xs-12 col-md-12">
                    <span className="custom-card-title-text">Titulación/es</span>
                    <p className="card-text ms-3">{content.grades ? content.grades.join("\r\n") : "-"}</p>
                  </div>
                </div>
                {/* <hr className="mt-1 mb-1" /> */}
                <div className="row">
                  <div className="col-xs-12 col-md-12">
                    <span className="custom-card-title-text">Etiquetas</span>
                    <br></br>
                    {content.tags && content.tags.map(tag => {
                      return (<span key={tag.name} className="badge text-bg-success mx-1">{tag.name}</span>)
                    })}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <ContentModal content={content} user={user} handleContent={handleContent}></ContentModal>
      </React.Fragment>
    )
  }
}