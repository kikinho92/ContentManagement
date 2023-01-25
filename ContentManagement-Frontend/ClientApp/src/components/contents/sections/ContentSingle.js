import React from "react";

import UrjcLogoNoWords from '../../../images/logo_urjc_nowords.png'

import { ContentModal } from "./ContentModal";

export class ContentSingle extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
    }
  }

  render() {

    const { content, user, handleContent } = this.props

    return (
      <React.Fragment>
        <div className="card mt-3">
          <div className="row no-gutters">
            <div className="col-xs-12 col-md-4">
              <img className="card-img img-thumbnail img-fluid rounded mx-auto d-block" src={UrjcLogoNoWords} width={150} alt="" style={{ width: "200px", position: "relative", top: "calc(50% - 100px)" }} />
            </div>
            <div className="col-xs-12 col-md-8">
              <div className="card-body">
                <h5 className="card-title">{content.title} <i className="bi bi-pencil-square float-end" data-bs-toggle="modal" data-bs-target={"#content-modal-" + content.id} style={{ cursor: "pointer" }}></i></h5>
                <div className="row">
                  <div className="col-xs-12 col-md-12">
                    <p className="card-text">{content.link}</p>
                  </div>
                </div>
                <hr className="mt-1 mb-1" />
                <div className="row">
                  <div className="col-xs-12 col-md-12">
                    <p className="card-text">{content.description}</p>
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
                    <p className="card-text">{content.licenseTypes.join(", ")}</p>
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