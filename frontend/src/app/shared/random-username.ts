const ADJECTIVES = ['brisk', 'calm', 'clever', 'eager', 'gentle', 'lively', 'quiet', 'swift', 'sunny', 'witty'];
const NOUNS = ['otter', 'falcon', 'maple', 'comet', 'harbor', 'meadow', 'pixel', 'raven', 'summit', 'willow'];

export function generateRandomUsername(): string {
  const adjective = ADJECTIVES[Math.floor(Math.random() * ADJECTIVES.length)];
  const noun = NOUNS[Math.floor(Math.random() * NOUNS.length)];
  const suffix = Math.floor(Math.random() * 9000) + 1000;
  return `${adjective}-${noun}-${suffix}`;
}
