import React from "react";

import CargaContenidosImg from "../../../images/carga_contenidos.png"

export class ContentModalExcelUpload extends React.Component {
  constructor(props) {
    super(props)
    this.state = {}
  }

  render() {
    return (
      <React.Fragment>
        
        <div className="modal fade" id="content-modal-excel" tabIndex={-1} aria-labelledby="content-modal-label" aria-hidden="true">
          <div className="modal-dialog modal-xl">
            <div className="modal-content custom-modal-content">
              <div className="modal-header">
                <h1 className="modal-title fs-5" id="content-modal-label">Carga de contenidos</h1>
              </div>
              <div className="modal-body">
                <div className="container-fluid">

                  <div className="row p-3">
                    <div className="col-md-12">
                      <p>

                      </p>
                      <img src={CargaContenidosImg} className="img-thumbnail" alt="..."></img>
                    </div>
                  </div>

                </div>
              </div>
            </div>
          </div>
        </div>

      </React.Fragment>
    )
  }
}