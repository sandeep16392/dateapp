import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit, AfterViewInit {
  model = {username: '', password: ''};
  photoUrl: string;
  @ViewChild('userRef') userEltRef: ElementRef;

  constructor(public authService: AuthService, private alertify: AlertifyService,
    private route: Router) {}

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  ngAfterViewInit(){
    this.userEltRef.nativeElement.focus();
  }

  login() {
    this.authService.login(this.model).subscribe(
      next => {
        this.alertify.success('Logged In!');
      },
      error => {
        this.alertify.error(error);
      }, () => {
        this.route.navigate(['/members']);
      }    );
  }

  loggedIn() {

    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    this.authService.currentUser = null;
    this.authService.decodedToken = null;
    this.alertify.message('Logged Out!');
    this.route.navigate(['/home']);
  }
}
