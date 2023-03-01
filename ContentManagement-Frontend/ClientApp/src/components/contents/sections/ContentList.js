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

  /* componentDidMount() {
    this.setState({ contents: this.props.contents }, () => {console.log("contents-contentlist", this.state.contents)})
  } */

  componentDidUpdate(prevProps) {
    if (prevProps === this.props) return;
    this.setState({ contents: this.props.contents })
  }

  render() {
    return (
      <React.Fragment>
        <div className="row">
          <div className="col-xs-12 col-md-12">
            {this.state.contents.map(content => {
              return ( <ContentSingle key={content.id} content={content} user={this.props.user} handleContent={this.props.handleContent}></ContentSingle>)
            })}
          </div>
        </div>

      </React.Fragment>
    )
  }
}