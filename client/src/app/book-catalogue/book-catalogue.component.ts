import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EntriesService } from '../entries.service';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { Book } from '../models/book.model';
import { AddBookModalComponent } from '../add-book-modal/add-book-modal.component';

@Component({
  selector: 'app-book-catalogue',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AddBookModalComponent],
  templateUrl: './book-catalogue.component.html',
  styleUrls: ['./book-catalogue.component.css']
})
export class BookCatalogueComponent implements OnInit {
  books: Book[] = [];
  errorMessage: string | null = null;
  showAddBookModal = false;
  isDeletingBook = false;
  isEditMode = false;
  selectedBook: Book | null = null;
  
  constructor(
    private entriesService: EntriesService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
        this.router.navigate(['/login']);
    }
    this.loadBooks();
  }
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  loadBooks(): void {
    this.entriesService.getEntries()
      .subscribe({
        next: (data: Book[]) => {
          this.books = data;
        },
        error: (err) => {
          this.errorMessage = `Failed to load books. Please try again later. Error: ${err}`;
        }
      });
  }

  handleBookAdded(book: Book): void {
    this.books.push(book);
    this.showAddBookModal = false;
  }

  handleBookUpdated(updatedBook: Book): void {
    const index = this.books.findIndex(book => book.publicId === updatedBook.publicId);
    if (index !== -1) {
      this.books[index] = updatedBook;
    }
    this.showAddBookModal = false;
    this.isEditMode = false;
    this.selectedBook = null;
  }

  handleModalClose(): void {
    this.showAddBookModal = false;
    this.isEditMode = false;
    this.selectedBook = null;
  }

  showAddBookForm(): void {
    this.isEditMode = false;
    this.selectedBook = null;
    this.showAddBookModal = true;
  }

  showEditBookForm(book: Book): void {
    this.isEditMode = true;
    this.selectedBook = book;
    this.showAddBookModal = true;
  }

  deleteBook(publicId: string): void {
    if (this.isDeletingBook) return;
    
    this.isDeletingBook = true;
    this.entriesService.deleteEntry(publicId)
      .subscribe({
        next: () => {
          this.books = this.books.filter(book => book.publicId !== publicId);
          this.isDeletingBook = false;
        },
        error: (err) => {
          this.errorMessage = `Failed to delete book. Please try again later. Error: ${err}`;
          this.isDeletingBook = false;
        }
      });
  }
}
