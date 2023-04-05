export function isDigit(c) {
  return typeof c === 'string' && c.trim() !== '' && !isNaN(Number(c))
}

export function isIdentifier(s) {
  return typeof s === 'string' && s.length && !isDigit(s[0]) && [...s].every(c => isLetterOrDigit(c) || c === '_')
}

export function isLetter(c) {
  return typeof c === 'string' && c.toLowerCase() !== c.toUpperCase()
}

export function isLetterOrDigit(c) {
  return typeof c === 'string' && (isLetter(c) || isDigit(c))
}
