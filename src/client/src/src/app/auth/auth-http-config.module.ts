import { AuthModule , LogLevel } from 'angular-auth-oidc-client';
import { environment }           from 'src/environments/environment';
import { NgModule }              from '@angular/core';

@NgModule( {
               imports : [
                   AuthModule.forRoot( {
                                        config: { authority             : environment.authority
                                                , redirectUrl           : window.location.origin
                                                , postLogoutRedirectUri : window.location.origin
                                                , clientId              : 'prometheus_client'
                                                , scope                 : 'openid profile email offline_access'
                                                , responseType          : 'code'
                                                , silentRenew           : true
                                                , useRefreshToken       : true
                                                , logLevel              : LogLevel.Debug
                                                }
                                       } )
               ] ,
               exports : [ AuthModule ]
           } )
export class AuthHttpConfigModule {
}
