const contentType = 'Content-Type'

async function execute(method, url, data = null) {
  const request = {
    headers: {},
    method
  }
  if (data) {
    request.body = JSON.stringify(data)
    request.headers[contentType] = 'application/json; charset=UTF-8'
  }
  const response = await fetch(url, request)
  const result = { data: null, status: response.status }
  const dataType = response.headers.get(contentType)
  if (dataType) {
    if (dataType.includes('json')) {
      result.data = await response.json()
    } else {
      throw new Error(`The content type "${dataType}" is not supported.`)
    }
  }
  if (!response.ok) {
    if (response.status === 401) {
      window.location.replace(`/user/sign-in?returnUrl=${window.location.pathname}`)
    }
    throw result
  }
  return result
}

export async function _delete(url) {
  return await execute('DELETE', url)
}

export async function get(url) {
  return await execute('GET', url)
}

export async function patch(url, data = null) {
  return await execute('PATCH', url, data)
}

export async function post(url, data = null) {
  return await execute('POST', url, data)
}

export async function put(url, data = null) {
  return await execute('PUT', url, data)
}
