import React, { useState } from 'react';
import { shortenUrl } from '../services/apiServices';
import ShortenedUrl from './ShortenedUrl';

function Shortener() {
    const [shortUrl, setShortUrl] = useState('');
    const [url, setUrl] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const data = await shortenUrl(url);
            const shortenedUrl = data.ShortUrl;
            setShortUrl(shortenedUrl);
        } catch (error) {
            console.error('Error shortening URL:', error);
            alert('An error occurred while shortening the URL: ' + error.message);
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

            {shortUrl && <ShortenedUrl url={shortUrl} />}
        </div>
    );
}

export default Shortener;
