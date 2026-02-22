import { NgModule }              from '@angular/core';
import { MatButtonModule }       from '@angular/material/button';
import { MatCardModule }         from '@angular/material/card';
import { MatCheckboxModule }     from '@angular/material/checkbox';
import { MatDatepickerModule }   from '@angular/material/datepicker';
import { MatDialogModule }       from '@angular/material/dialog';
import { MatExpansionModule }    from '@angular/material/expansion';
import { MatFormFieldModule }    from '@angular/material/form-field';
import { MatIconModule }         from '@angular/material/icon';
import { MatInputModule }        from '@angular/material/input';
import { MatNativeDateModule }   from '@angular/material/core';
import { MatPaginatorModule }    from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule }       from '@angular/material/select';
import { MatTableModule }        from '@angular/material/table';
import { MatToolbarModule }      from '@angular/material/toolbar';
import { MatTooltipModule }      from '@angular/material/tooltip';
import { CommonModule }          from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

const MATERIAL = [
  CommonModule, FormsModule, ReactiveFormsModule,
  MatButtonModule, MatCardModule, MatCheckboxModule,
  MatDatepickerModule, MatDialogModule, MatExpansionModule,
  MatFormFieldModule, MatIconModule, MatInputModule,
  MatNativeDateModule, MatPaginatorModule, MatProgressSpinnerModule,
  MatSelectModule, MatTableModule, MatToolbarModule, MatTooltipModule
];

@NgModule({ imports: MATERIAL, exports: MATERIAL })
export class SharedMaterialModule {}
