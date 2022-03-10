import { AppComponent }                   from './app.component';
import { AppRoutingModule }               from './app-routing.module';
import { AuthModule }                     from '@auth0/auth0-angular';
import { BrowserAnimationsModule }        from '@angular/platform-browser/animations';
import { BrowserModule }                  from '@angular/platform-browser';
import { NbEvaIconsModule }               from '@nebular/eva-icons';
import { NbThemeModule, NbLayoutModule }  from '@nebular/theme';
import { NgModule }                       from '@angular/core';

@NgModule( {
  declarations: [
    AppComponent
  ] 
  , imports: [
    AppRoutingModule
  , AuthModule.forRoot( { domain   : 'soprom.eu.auth0.com'
                        , clientId : 'ePJBJoRCKnZWwpls6gNSJRT9LdzOoOKP' } )
  , BrowserAnimationsModule
  , BrowserModule
  , NbEvaIconsModule
  , NbLayoutModule
  , NbThemeModule.forRoot( { name: 'default' } )
  ]
  , providers: []
  , bootstrap: [ AppComponent ]
} )
export class AppModule { }
