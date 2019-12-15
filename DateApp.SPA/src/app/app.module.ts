import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule } from 'ngx-bootstrap';
import { JwtModule } from '@auth0/angular-jwt';
import { RouterModule } from '@angular/router';
import { NgxGalleryModule } from 'ngx-gallery';
import { TimeAgoPipe } from 'time-ago-pipe';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './services/auth.service';
import { RegisterComponent } from './register/register.component';
import { HomeComponent } from './home/home.component';
import { ErrorInterceptorProvider } from './services/error.interceptor';
import { AlertifyService } from './services/alertify.service';
import { MemberListComponent } from './member/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { appRoutes } from './routes';
import { AuthGuard } from './guards/auth.guard';
import { MemberCardComponent } from './member/member-card/member-card.component';
import { MemberDetailComponent } from './member/member-detail/member-detail.component';

import { environment } from 'src/environments/environment';
import { MemberDetailResolver } from './resolver/memberdetail.resolver';
import { MemberListResolver } from './resolver/memberlist.resolver';
import { MemberEditComponent } from './member/member-edit/member-edit.component';
import { MemberEditResolver } from './resolver/memberedit.resolver';
import { PreventUnsavedChangesGuard } from './guards/prevent-unsaved-changes.guard';
import { PhotoEditorComponent } from './member/photo-editor/photo-editor.component';
import { FileUploadModule } from 'ng2-file-upload';
import { ListsResolver } from './resolver/lists.resolver';
import { MessagesResolver } from './resolver/messages.resolver';
import { MemberMessagesComponent } from './member/member-messages/member-messages.component';

import * as Sentry from '@sentry/browser';

Sentry.init({
  dsn: 'https://9ee19f4da1bb41ea85f68eaca429ca42@sentry.io/1838241'
});

export function tokenGetter() {
   return localStorage.getItem('token');
};

@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      RegisterComponent,
      HomeComponent,
      MemberListComponent,
      MessagesComponent,
      ListsComponent,
      MemberCardComponent,
      MemberDetailComponent,
      MemberEditComponent,
      PhotoEditorComponent,
      TimeAgoPipe,
      MemberMessagesComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      NgxGalleryModule,
      FileUploadModule,
      BsDropdownModule.forRoot(),
      RouterModule.forRoot(appRoutes),
      TabsModule.forRoot(),
      JwtModule.forRoot({
         config: {
            tokenGetter: tokenGetter,
            whitelistedDomains: environment.whiteListedDomain,
            blacklistedRoutes: environment.blacklistDomain
         }
      }),
      BsDatepickerModule.forRoot(),
      PaginationModule.forRoot(),
      ButtonsModule.forRoot()
   ],
   providers: [
      AuthService,
      ErrorInterceptorProvider,
      AlertifyService,
      AuthGuard,
      MemberDetailResolver,
      MemberListResolver,
      MemberEditResolver,
      PreventUnsavedChangesGuard,
      ListsResolver,
      MessagesResolver
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
