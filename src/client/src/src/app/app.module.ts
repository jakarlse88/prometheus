import { AppComponent }                   from './app.component';
import { AppRoutingModule }               from './app-routing.module';
import { AuthHttpConfigModule }           from './auth/auth-http-config.module';
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
  , AuthHttpConfigModule
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
