import React from "react";
import { Card, CardBody } from "reactstrap";

const Video = ({ video }) => {
  return (
    <Card >
      <p className="text-left px-2">Posted by: {video.userProfile.name}</p>
      <CardBody>
        <iframe className="video"
          src={video.url}
          title="YouTube video player"
          frameBorder="0"
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
          allowFullScreen />

        <p>
          <strong>{video.title}</strong>
        </p>
        <p>{video.description}</p>
        <p><h4>Comments</h4></p>
        <div className="video-comments">
            {video.comments.length !== 0 ?
                video.comments.map(c => {
                    return <p className="video-comment">
                        {`${c.userProfile.name} says: `} {`"${c.message}"`}
                    </p>})
                 : 
                 <p>None</p>}
        </div>
      </CardBody>
    </Card>
  );
};

export default Video;
