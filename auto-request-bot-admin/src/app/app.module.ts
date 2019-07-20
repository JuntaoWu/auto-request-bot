import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
  MatTableModule, MatPaginatorModule, MatButtonModule, MatToolbarModule, MatDatepickerModule, MatNativeDateModule,
  MatFormFieldModule,
  MatInputModule,
  MatDividerModule,
  MatSelectModule,
  MatDialogModule,
  MatSnackBarModule
} from '@angular/material';

import { AppComponent } from './app.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MemberDetailComponent } from './member-detail/member-detail.component';
import { DescribeStatusPipe } from './describe-status.pipe';
import { DescribeLocationPipe } from './describe-location.pipe';
import { AuthHttpInterceptor } from './http-interceptor.service';
import { AuthStoreService } from './auth-store.service';

@NgModule({
  declarations: [
    AppComponent,
    MemberDetailComponent,
    DescribeStatusPipe,
    DescribeLocationPipe
  ],
  imports: [
    FormsModule,
    BrowserModule,
    HttpClientModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatToolbarModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule,
    MatFormFieldModule,
    MatDividerModule,
    MatSelectModule,
    MatFormFieldModule,
    BrowserAnimationsModule,
    MatDialogModule,
    MatSnackBarModule,
  ],
  providers: [
    AuthStoreService,
    { provide: HTTP_INTERCEPTORS, useClass: AuthHttpInterceptor, multi: true }
  ],
  bootstrap: [AppComponent],
  entryComponents: [
    MemberDetailComponent,
  ]
})
export class AppModule { }
