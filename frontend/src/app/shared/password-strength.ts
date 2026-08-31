export interface PasswordStrength {
  score: 0 | 1 | 2 | 3 | 4;
  label: 'Very weak' | 'Weak' | 'Fair' | 'Good' | 'Strong';
  color: string;
}

const LABELS: PasswordStrength['label'][] = ['Very weak', 'Weak', 'Fair', 'Good', 'Strong'];
const COLORS = ['#e74c3c', '#e67e22', '#f1c40f', '#2ecc71', '#27ae60'];

export function computePasswordStrength(password: string): PasswordStrength {
  if (!password) {
    return { score: 0, label: LABELS[0], color: COLORS[0] };
  }

  let score = 0;
  if (password.length >= 6) score++;
  if (password.length >= 10) score++;
  if (/[a-z]/.test(password) && /[A-Z]/.test(password)) score++;
  if (/\d/.test(password)) score++;
  if (/[^A-Za-z0-9]/.test(password)) score++;

  const clamped = Math.min(4, Math.max(0, score - 1)) as PasswordStrength['score'];
  return { score: clamped, label: LABELS[clamped], color: COLORS[clamped] };
}
