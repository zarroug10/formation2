import { Component, inject, input, OnInit, output } from '@angular/core';
import { Member } from '../../_models/Member';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { Photo } from '../../_models/Photo';
import { MembersService } from '../../_services/members.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [NgClass, NgIf, NgStyle, NgFor, FileUploadModule, DecimalPipe],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css'
})
export class PhotoEditorComponent implements OnInit {


  private accountservice = inject(AccountService);
  private memberservice = inject(MembersService);
  private toatr = inject(ToastrService);

  member = input.required<Member>();
  uploader?: FileUploader;
  hasBaseDropZoneOver = false;
  basedUrl = environment.apiUrl;
  memberChange = output<Member>();


  ngOnInit(): void {
    this.initializeUploader();
  }
  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e
  }
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.basedUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.accountservice.currentUser()?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false
    }
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      const photo = JSON.parse(response);
      const updatedMember = { ...this.member() }
      updatedMember.photos.push(photo);
      this.memberChange.emit(updatedMember);
      if(photo.isMain)
      {
        const user = this.accountservice.currentUser();
        if (user) {
          user.photoUrl = photo.url;
          this.accountservice.setCurrentUser(user)
        }
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach(p => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
          this.memberChange.emit(updatedMember)
        });
      }
    }
  }
  DeletePicture(photo: Photo) {
    this.memberservice.deletePhoto(photo).subscribe({
      next: _ => {
        const updateMember = {...this.member()};
        updateMember.photos = updateMember.photos.filter(x=> x.id !== photo.id);
        this.memberChange.emit(updateMember);
        console.log('photo Deleted')
        this.toatr.success('photo Deleted')
      },
      error: error => {
        console.log(error.error);
      }
    })
  }

  setMain(photo: Photo) {
    this.memberservice.UpdateMainPhoto(photo).subscribe({
      next: _ => {
        const user = this.accountservice.currentUser();
        if (user) {
          user.photoUrl = photo.url;
          this.accountservice.setCurrentUser(user)
        }
        const updatedMember = { ...this.member() }
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach(p => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
          this.memberChange.emit(updatedMember)
        });

      }
    })
  }


}
