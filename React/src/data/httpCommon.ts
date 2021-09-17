import axios from 'axios'

export default axios.create({
  baseURL: 'https://localhost:44399/api/v1',
  headers: {
    'Content-type': 'application/json',
  },
})
