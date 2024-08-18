import axios from 'axios';

const API_URL = 'http://localhost:5555/Url/';

export const shortenUrl = async (originalUrl) => {
    try {
        const response = await axios.post(`${API_URL}/`, { url: originalUrl });
        return response.data;
    } catch (error) {
        // Extract relevant error information
        const errorMessage = error.response?.data?.message || error.message;
        const errorCode = error.response?.status || 500;

        // Throw a custom error with more details
        const customError = new Error(`Error shortening URL: ${errorMessage}`);
        customError.code = errorCode;
        throw customError;
    }
};


export const getOriginalUrl = async (shortCode) => {
    try {
        const response = await axios.get(`${API_URL}/${shortCode}`);
        return response.data;
    } catch (error) {
        console.error('Error resolving URL:', error);
        throw error;
    }
};

