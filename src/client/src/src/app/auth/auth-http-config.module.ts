import { AuthModule , StsConfigHttpLoader , StsConfigLoader } from 'angular-auth-oidc-client';
import { environment }                                        from 'src/environments/environment';
import { HttpClient }                                         from '@angular/common/http';
import { map }                                                from 'rxjs/operators';
import { NgModule }                                           from '@angular/core';

export const httpLoaderFactory = ( httpClient : HttpClient ) => {
    const config$ = httpClient
            .get<any>( `${ environment.oidcUrl }/.well-known/openid-configuration` )
            .pipe(
                    map( ( customConfig : any ) => {
                        return {
                            authority                           : customConfig.authority ,
                            redirectUrl                         : customConfig.redirect_url ,
                            clientId                            : customConfig.client_id ,
                            responseType                        : customConfig.response_type ,
                            scope                               : customConfig.scope ,
                            postLogoutRedirectUri               : customConfig.post_logout_redirect_uri ,
                            startCheckSession                   : customConfig.start_checksession ,
                            silentRenew                         : customConfig.silent_renew ,
                            silentRenewUrl                      : customConfig.redirect_url + '/silent-renew.html' ,
                            postLoginRoute                      : customConfig.startup_route ,
                            forbiddenRoute                      : customConfig.forbidden_route ,
                            unauthorizedRoute                   : customConfig.unauthorized_route ,
                            logLevel                            : customConfig.logLevel , // LogLevel.Debug,
                            maxIdTokenIatOffsetAllowedInSeconds : customConfig.max_id_token_iat_offset_allowed_in_seconds ,
                            historyCleanupOff                   : true
                            // autoUserInfo: false,
                        };
                    } )
            );
            // .toPromise();

    return new StsConfigHttpLoader( config$ );
};

@NgModule( {
               imports : [
                   AuthModule.forRoot( {
                                           loader : {
                                               provide    : StsConfigLoader ,
                                               useFactory : httpLoaderFactory ,
                                               deps       : [ HttpClient ]
                                           }
                                       } )
               ] ,
               exports : [ AuthModule ]
           } )
export class AuthHttpConfigModule {
}
