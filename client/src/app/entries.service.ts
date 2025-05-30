import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Book } from './models/book.model';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EntriesService {
  private apiUrl = environment.apiUrl;
  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient) { }

  getEntries(): Observable<Book[]> {
    return this.http.get<Book[]>(`${this.apiUrl}/Entries`);
  }

  addEntry(book: Book): Observable<Book> {
    return this.http.post<Book>(`${this.apiUrl}/Entries`, book, this.httpOptions);
  }

  updateEntry(publicId: string, book: Book): Observable<Book> {
    return this.http.put<Book>(`${this.apiUrl}/Entries/${publicId}`, book, this.httpOptions);
  }

  deleteEntry(publicId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Entries/${publicId}`);
  }
}
