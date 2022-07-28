export function isDigit(c) {
  return c.trim() !== '' && !isNaN(Number(c))
}

export function isLetter(c) {
  return c.toLowerCase() !== c.toUpperCase()
}

export function isLetterOrDigit(c) {
  return isLetter(c) || isDigit(c)
}
