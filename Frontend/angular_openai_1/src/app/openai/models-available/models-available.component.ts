import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-models-available',
  imports: [CommonModule, FormsModule],
  templateUrl: './models-available.component.html',
  styleUrl: './models-available.component.css'
})
export class ModelsAvailableComponent  implements OnInit {
  httpClient = inject(HttpClient);
  public jsonResult: Result | null = null;

  selectedModel: string = '';

  ngOnInit(): void {
    this.httpClient.get('https://localhost:7163/api/chat/models')
            .subscribe({
              next: (data: any) => 
              {
                console.log(data);
                this.jsonResult = data;

                // Obtiene el valor guadado de un modelo previamente seleccionado
                const storedModel = localStorage.getItem('selectedModel');
                if (storedModel)
                {
                  this.selectedModel = storedModel;
                }
              },
              error: (error) => console.error(error)
            });
  }

  onSelectedModel(event: Event)
  {
    const selectedElement = event.target as HTMLSelectElement;
    this.selectedModel = selectedElement.value;

    // Guardar modelo en localstorage
    localStorage.setItem('selectedModel', this.selectedModel);
  }
}

export interface Model {
  name: string;
}

export interface Result {
  result: Model[];
}
