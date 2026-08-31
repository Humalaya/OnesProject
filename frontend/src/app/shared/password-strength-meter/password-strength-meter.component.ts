import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { computePasswordStrength } from '../password-strength';

@Component({
  selector: 'app-password-strength-meter',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './password-strength-meter.component.html',
  styleUrls: ['./password-strength-meter.component.scss']
})
export class PasswordStrengthMeterComponent {
  @Input() password = '';

  get strength() {
    return computePasswordStrength(this.password);
  }
}
