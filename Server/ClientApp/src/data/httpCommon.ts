import axios from 'axios'

export default axios.create({
  baseURL: process.env.NODE_ENV === 'development'
    ? 'https://localhost:5001/api/v1'
    : 'https://rootool.rcapps.ca/api/v1', 
  headers: {
    'Content-type': 'application/json',
  },
})
