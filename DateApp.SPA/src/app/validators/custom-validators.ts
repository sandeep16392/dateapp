import { FormGroup } from '@angular/forms';

export class CustomValidators {

    passwordMatchCheck(g: FormGroup) {
        return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
    }
}
