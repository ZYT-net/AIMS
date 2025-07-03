import axios from 'axios'
import { getToken } from '@/utils/auth'

const instance = axios.create({
  baseURL: '/api',
  timeout: 10000
})

instance.interceptors.request.use(config => {
  const token = getToken()
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

instance.interceptors.response.use(
  res => res,
  err => {
    // 可根据后端返回的状态码做全局处理
    if (err.response && err.response.status === 401) {
      // 跳转登录或提示
    }
    return Promise.reject(err)
  }
)

export default instance
