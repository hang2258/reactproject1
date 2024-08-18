import React, { useEffect, useState } from 'react';

function UrlList() {
    const [urls, setUrls] = useState([]);

    useEffect(() => {
        fetch('http://localhost:7077/api/url/GetAllUrls')
            .then(response => response.json())
            .then(data => setUrls(data))
            .catch(error => console.error('Error fetching URLs:', error));
    }, []);

    return (
        <div>
            <h1>Shortened URLs</h1>
            <ul>
                {urls.map(url => (
                    <li key={url.id}>
                        <strong>Original URL:</strong> {url.originalUrl} <br />
                        <strong>Short URL:</strong> {url.shortUrl} <br />
                        <strong>Created At:</strong> {new Date(url.createdAt).toLocaleString()} <br />
                        {url.expirationDate && (
                            <><strong>Expiration Date:</strong> {new Date(url.expirationDate).toLocaleString()}<br /></>
                        )}
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default UrlList;
