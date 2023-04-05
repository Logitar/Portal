export function getQueryString(params) {
  const filtered = Object.entries(params).filter(([, value]) => typeof value !== 'undefined' && value !== null)

  const list = []
  filtered.forEach(([key, value]) => {
    if (Array.isArray(value)) {
      value.forEach(item => list.push([key, item]))
    } else {
      list.push([key, value])
    }
  })

  return '?' + list.map(pair => pair.join('=')).join('&')
}
