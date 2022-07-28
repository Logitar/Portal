const allowedUserNameCharacters = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+'

export default function (value) {
  return typeof value === 'string' && [...value].every(c => allowedUserNameCharacters.includes(c))
}
