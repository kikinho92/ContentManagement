import React from "react";

import { ContentModal } from "./ContentModal";

import UserCli from "../../../sdk/UserCli";

const URJC = "UNIVERSIDAD REY JUAN CARLOS (MÓSTOLES)"
const UC3M = "UNIVERSIDAD CARLOS III (LEGANÉS)"

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
          this.setState({ userContentError: response.substring(session.indexOf(' ')) })
        } else {
          var user = response
          console.log(user)
          var logo = "";
          switch (user.group.name.toUpperCase()) {
            case URJC:
              logo = "custom-urjc-image"
              break;
            case UC3M:
              logo = "custom-uc3m-image"
              break;
            default:
              break;
          }
          this.setState({ ownerLogo: logo })
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
                <h5 className="card-title">{content.title} <i className="bi bi-pencil-square float-end" data-bs-toggle="modal" data-bs-target={"#content-modal-" + content.id} style={{ cursor: "pointer" }}></i></h5>
                <div className="row">
                  <div className="col-xs-12 col-md-12">
                    <p className="card-text"><a href={content.link} target="_blank">Link del contenido</a></p>
                  </div>
                </div>
                <hr className="mt-1 mb-1" />
                <div className="row">
                  <div className="col-xs-12 col-md-12">
                    <p className="card-text">{content.description ? content.description : "-"}</p>
                  </div>
                </div>
                <hr className="mt-1 mb-1" />
                <div className="row">
                  <div className="col-xs-12 col-md-12">
                    <p className="card-text">{content.authors.join(", ")}</p>
                  </div>
                </div>
                <hr className="mt-1 mb-1" />
                <div className="row">
                  <div className="col-xs-12 col-md-12">
                    <p className="card-text">{content.department ? content.department : "-"}</p>
                  </div>
                </div>
                <hr className="mt-1 mb-1" />
                <div className="row">
                  <div className="col-xs-12 col-md-12">
                    <p className="card-text">{content.grades.length > 0 ? content.grades.join(", ") : "-"}</p>
                  </div>
                </div>
                <hr className="mt-1 mb-1" />
                <div className="row">
                  <div className="col-xs-12 col-md-12">
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