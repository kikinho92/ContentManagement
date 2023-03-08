import React from "react";

import { ContentSingle } from "./ContentSingle";

export class ContentList extends React.Component {
  constructor(props) {
    super(props)
    this.state = {
      contents: props.contents,

      openModal: false
    }
  }

  componentDidUpdate(prevProps) {
    if (prevProps === this.props) return;
    this.setState({ contents: this.props.contents })
  }

  render() {
    return (
      <React.Fragment>
        <div className="row pt-3 mt-5">
          {/* <div className="col-xs-12 col-md-12"> */}
            {this.state.contents.map(content => {
              return (
                <div key={content.id} className="col-xs-12 col-sm-12 col-md-6">
                  <ContentSingle content={content} user={this.props.user} handleContent={this.props.handleContent}></ContentSingle>
                </div>
              )
            })}
          {/* </div> */}
        </div>

      </React.Fragment>
    )
  }
}