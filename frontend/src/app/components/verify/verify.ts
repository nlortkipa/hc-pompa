import { Component, ElementRef, QueryList, ViewChildren } from '@angular/core';
import { RouterLink } from "@angular/router";

@Component({
  standalone: true,
  selector: 'app-verify',
  imports: [RouterLink],
  templateUrl: './verify.html',
  styleUrl: './verify.scss',
})
export class Verify {
  @ViewChildren('otpInput')
  otpInputs!: QueryList<ElementRef<HTMLInputElement>>;

  onOtpInput(event: Event, index: number) {
    const input = event.target as HTMLInputElement;

    input.value = input.value.replace(/\D/g, '');

    if (input.value && index < this.otpInputs.length - 1) {
      this.otpInputs.get(index + 1)?.nativeElement.focus();
    }
  }

  onKeyDown(event: KeyboardEvent, index: number) {
    const input = event.target as HTMLInputElement;

    if (event.key === 'Backspace' && !input.value && index > 0) {
      this.otpInputs.get(index - 1)?.nativeElement.focus();
    }
  }

  onPaste(event: ClipboardEvent) {
    event.preventDefault();

    const pasted = event.clipboardData
      ?.getData('text')
      .replace(/\D/g, '')
      .slice(0, this.otpInputs.length);

    if (!pasted) return;

    pasted.split('').forEach((char, i) => {
      const input = this.otpInputs.get(i)?.nativeElement;

      if (input) {
        input.value = char;
      }
    });

    this.otpInputs.get(pasted.length - 1)?.nativeElement.focus();
  }
}
