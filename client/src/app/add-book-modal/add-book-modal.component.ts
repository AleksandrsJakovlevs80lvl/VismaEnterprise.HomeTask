import { Component, EventEmitter, Input, OnInit, Output, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { EntriesService } from '../entries.service';
import { Book } from '../models/book.model';

@Component({
  selector: 'app-add-book-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-book-modal.component.html',
  styleUrls: ['./add-book-modal.component.css']
})
export class AddBookModalComponent implements OnInit, OnChanges {
  @Input() isVisible: boolean = false;
  @Input() editMode: boolean = false;
  @Input() bookToEdit: Book | null = null;
  @Output() closeModal = new EventEmitter<void>();
  @Output() bookAdded = new EventEmitter<Book>();
  @Output() bookUpdated = new EventEmitter<Book>();

  addBookForm: FormGroup;
  addBookError: string | null = null;
  modalTitle: string = 'Add New Book';
  submitButtonText: string = 'Add Book';

  constructor(private fb: FormBuilder, private entriesService: EntriesService) {
    this.addBookForm = this.fb.group({
      Title: ['', Validators.required],
      Author: ['', Validators.required],
      PublishedYear: [null as number | null],
      Genre: [''],
      Mark: [null as number | null]
    });
  }
  
  ngOnInit(): void {
    // Add event listener to close modal on ESC key
    document.addEventListener('keydown', (event) => {
      if (event.key === 'Escape' && this.isVisible) {
        this.closeModal.emit();
      }
    });
  }
  ngOnChanges(changes: SimpleChanges): void {
    // Update form when bookToEdit changes
    if (changes['bookToEdit'] && this.bookToEdit) {
      this.populateFormWithBook(this.bookToEdit);
    }

    // Update modal title and button text based on editMode
    if (changes['editMode']) {
      this.modalTitle = this.editMode ? 'Edit Book' : 'Add New Book';
      this.submitButtonText = this.editMode ? 'Edit Book' : 'Add Book';
      
      // Update form controls accessibility based on edit mode
      if (this.editMode) {
        this.addBookForm.get('Title')?.disable();
        this.addBookForm.get('Author')?.disable();
      } else {
        this.addBookForm.get('Title')?.enable();
        this.addBookForm.get('Author')?.enable();
      }
    }
  }
  populateFormWithBook(book: Book): void {
    // Use setValue to ensure all form controls are populated, even disabled ones
    this.addBookForm.patchValue({
      Title: book.title,
      Author: book.author,
      PublishedYear: book.publishedYear || null,
      Genre: book.genre || '',
      Mark: book.mark || null
    }, { emitEvent: false });
  }
  resetForm(): void {
    // Enable all controls before resetting
    this.addBookForm.get('Title')?.enable();
    this.addBookForm.get('Author')?.enable();
    
    this.addBookForm.reset({
      Title: '',
      Author: '',
      PublishedYear: null,
      Genre: '',
      Mark: null
    });
  }
  onAddBookSubmit(): void {
    this.addBookError = null;
    if (this.addBookForm.valid) {
      const formValue = this.addBookForm.getRawValue(); // getRawValue includes disabled controls
      
      if (this.editMode && this.bookToEdit) {
        const updatedBook: Book = {
          ...this.bookToEdit,
          // In edit mode, title and author remain unchanged
          publishedYear: formValue.PublishedYear,
          genre: formValue.Genre,
          mark: formValue.Mark
        };
        
        this.entriesService.updateEntry(this.bookToEdit.publicId!, updatedBook).subscribe({
          next: (updatedBook) => {
            this.bookUpdated.emit(updatedBook);
            this.resetForm();
          },
          error: (err) => {
            console.error('Error updating book:', err);
            this.addBookError = 'Failed to update book. Please try again later.';
            if (err.status === 401) {
              this.addBookError = 'Unauthorized. Please login again.';
            }
          }
        });
      } else {
        const newBook: Book = {
          publicId: undefined,
          title: formValue.Title,
          author: formValue.Author,
          publishedYear: formValue.PublishedYear,
          genre: formValue.Genre,
          mark: formValue.Mark
        };
        
        this.entriesService.addEntry(newBook).subscribe({
          next: (addedBook) => {
            this.bookAdded.emit(addedBook);
            this.resetForm();
          },
          error: (err) => {
            console.error('Error adding book:', err);
            this.addBookError = 'Failed to add book. Please try again later.';
            if (err.status === 401) {
              this.addBookError = 'Unauthorized. Please login again.';
            }
          }
        });
      }
    } else {
      this.addBookForm.markAllAsTouched();
    }
  }
}
