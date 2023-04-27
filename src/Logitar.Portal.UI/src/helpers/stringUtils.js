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

export function shortify(text, length) {
  return text?.length > length ? text.substring(0, length - 1) + '…' : text
}

export function slugify(value) {
  value = value ?? ''
  const words = []
  let word = ''
  for (let i = 0; i < value.length; i++) {
    const c = value[i]
    if (isLetterOrDigit(c)) {
      word += c
    } else if (word.length) {
      words.push(word)
      word = ''
    }
  }
  if (word.length) {
    words.push(word)
  }
  return unaccent(words.join('-').toLowerCase())
}

export function unaccent(input) {
  const accents = {
    à: 'a',
    â: 'a',
    ç: 'c',
    è: 'e',
    é: 'e',
    ê: 'e',
    ë: 'e',
    î: 'i',
    ï: 'i',
    ô: 'o',
    ù: 'u',
    û: 'u',
    ü: 'u'
  }
  return [...input].map(c => (c.toUpperCase() === c ? (accents[c] ?? c).toUpperCase() : accents[c] ?? c)).join('')
}
