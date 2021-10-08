import React, { useEffect, useState } from "react";
import Video from "./Video";
import {
    getAllVideosWithComments,
    searchVideos,
} from "../modules/videoManager";

const VideoList = () => {
    const [videos, setVideos] = useState([]);
    const [searchTerms, setSearchTerms] = useState("");

    const getVideos = () => {
        getAllVideosWithComments().then((videos) => setVideos(videos));
    };

    useEffect(() => {
        getVideos();
    }, []);

    const handleInputChange = (e) => {
        let searchTerms = e.target.value;
        setSearchTerms(searchTerms);
    };

    const handleSearchSubmit = () => {
        searchVideos(searchTerms, true)
        .then(results => {
            setVideos(results);
        });
        setSearchTerms("");
    };

    return (
        <>
            <div className="search-form">
                <input
                    onChange={handleInputChange}
                    placeholder="Search videos"
                    value={searchTerms}
                />
                <button type="button" onClick={handleSearchSubmit}>
                    Submit
                </button>
            </div>
            <div className="container">
                <div className="row justify-content-center">
                    {videos.map((video) => (
                        <Video video={video} key={video.id} />
                    ))}
                </div>
            </div>
        </>
    );
};

export default VideoList;
