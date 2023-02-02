export function generateSecret(length) {
  const buffer = new Uint8Array(length * 3)
  let bytes = []
  do {
    crypto.getRandomValues(buffer)
    bytes = buffer.filter(b => b > 32 && b < 127).slice(0, length)
  } while (bytes.length < 32)
  return String.fromCharCode(...bytes)
}
