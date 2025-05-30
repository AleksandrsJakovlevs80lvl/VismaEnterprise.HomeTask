import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { BookCatalogueComponent } from './book-catalogue/book-catalogue.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'catalogue', component: BookCatalogueComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];
