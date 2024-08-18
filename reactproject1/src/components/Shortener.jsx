import React, { useState, useEffect } from 'react';
import axios from 'axios';

const Shortener = () => {
    const [url, setUrl] = useState('');
    const [shortUrl, setShortUrl] = useState('');
    const [urlList, setUrlList] = useState([]);
    const [dropStatus, setDropStatus] = useState('');

    useEffect(() => {
        fetchUrlList();
    }, []);

    const fetchUrlList = async () => {
        try {
            const response = await axios.get('http://localhost:7077/api/Url/GetAllUrls');
            setUrlList(response.data);
        } catch (error) {
            console.error('Error fetching URL list:', error);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!url) {
            alert('Please enter a URL');
            return;
        }

        try {
            const response = await axios.post('http://localhost:7077/api/Url', { originalUrl: url });
            setShortUrl(response.data.shortUrl);
            fetchUrlList(); // Refresh the URL list after shortening
        } catch (error) {
            console.error('Error shortening URL:', error);
            alert('An error occurred while shortening the URL: ' + error.message);
        }
    };

    const handleDropTable = async () => {
        try {
            const response = await axios.delete('http://localhost:7077/api/Url/DropUrlTable');
            setDropStatus(response.data);
            fetchUrlList(); // Refresh the URL list after dropping the table
        } catch (error) {
            console.error('Error dropping the table:', error);
            alert('An error occurred while dropping the table: ' + error.message);
        }
    };

    return (
        <div className="shortener-container">
            <h1>URL Shortener</h1>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    value={url}
                    onChange={(e) => setUrl(e.target.value)}
                    placeholder="Enter URL"
                />
                <button type="submit">Shorten</button>
            </form>
            {shortUrl && (
                <div>
                    <p>Shortened URL:</p>
                    <a href={shortUrl} target="_blank" rel="noopener noreferrer">{shortUrl}</a>
                </div>
            )}
            <div>
                <h2>Shortened URLs List</h2>
                <ul>
                    {urlList.map((item) => (
                        <li key={item.id}>
                            <a href={item.shortUrl} target="_blank" rel="noopener noreferrer">
                                {item.shortUrl}
                            </a>
                            <p>Original URL: {item.originalUrl}</p>
                        </li>
                    ))}
                </ul>
            </div>

        </div>
    );
};

export default Shortener;
