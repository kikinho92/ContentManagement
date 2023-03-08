import React from "react";

import CargaContenidosImg from "../../../images/carga_contenidos.png"
import ContentCli from "../../../sdk/ContentCli";

export class ContentModalExcelUpload extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      uploadContentError: null
    }
  }

  handleFile = (event) => {
    var files = event.target.files

    var formData = new FormData()
    for (const file of files) {
      formData.append("files", file)
    }

    ContentCli.UploadContents(this.props.user.id, formData).then(response => {
      if (response !== null && response.toString().startsWith("ERROR")) {
        this.setState({ uploadContentError: response.substring(response.indexOf(' - ')) })
      } else {
        this.props.handleMultipleContents(response)
        new bootstrap.Modal(document.getElementById('content-modal-excel'), options).hide()
      }
    })
  }

  render() {
    return (
      <React.Fragment>
        
        <div className="modal fade" id="content-modal-excel" tabIndex={-1} aria-labelledby="content-modal-label" aria-hidden="true">
          <div className="modal-dialog modal-xl">
            <div className="modal-content custom-modal-content">
              <div className="modal-header">
                <h1 className="modal-title fs-5" id="content-modal-label">Carga de contenidos</h1>
                <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
              </div>
              <div className="modal-body">
                <div className="container-fluid">

                  <div className="row p-3">
                    <div className="col-md-12">
                      <p>
                        El contenido está formado por los siguientes campos (marcados con * los obligatorios) :
                      </p>
                      <ul>
                        <li><strong>Título *:</strong> Título del contenido.</li>
                        <li><strong>Link *:</strong> Enlace que redirige al detalle del contenido.</li>
                        <li><strong>Descripción:</strong> Explicación a modo de resumen acerca del contenido.</li>
                        <li><strong>Autor/es:</strong> Autor del contenido. Pueden ser uno o varios.</li>
                        <li><strong>Titulación/es:</strong> Titulación/es a las que va orientadas el contenido.</li>
                        <li><strong>Etiquetas:</strong> Etiquetas por las cuales se desea que el contenido sea identificado.</li>
                      </ul>
                      <p>
                        Para realizar la carga de uno o varios contenidos se deberá subir un archivo de Excel (.xls / .xlsx / .csv) con el formato que se expondrá a continuación:
                      </p>
                      <br></br>
                      <img src={CargaContenidosImg} className="img-thumbnail" alt="..."></img>
                      <br></br>
                      <p>
                        Como se puede observar en la imagen de ejemplo, cada fila representa un contenido y cada celda de la fila representa cada uno de sus campos. Las palabras
                        clave que se muestran en el ejemplo deberán ser sustituidas por la información del contenido a la que representan.
                      </p>
                      <p>
                        Para aquellos campos que permitan más de un valor (Autores, Titulaciones o Etiquetas), cada uno de los valores deberá usar como separador <strong>";"</strong> (Ej: Autor1;
                        Autor2;Autor3)
                      </p>
                      <p>
                        De este modo, un contenido ocupará 1 fila y sus campos ocuparán 6 celdas en total. Si cualquiera de las premisas expuestas en este tutorial no
                        es cumplida, se devolverá un <strong>Error de Formato</strong>.
                      </p>
                    </div>
                  </div>
                  <div className="row p-3">
                    <div className="col-xs-12 col-md-3 offset-md-4">
                      <label htmlFor="upload-content-file" className="btn btn-success custom-btn-success">
                        Cargar fichero de contenidos
                        <input className="form-control" type="file" id="upload-content-file" accept=".xlsx, .xls, .csv" style={{ display: "none" }} onChange={this.handleFile}/>
                      </label>
                      
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