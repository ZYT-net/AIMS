import request from './index'

export function login(data) {
  return request.post('/users/login', data)
}
