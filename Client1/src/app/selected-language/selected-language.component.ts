import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { TaskService } from '../services/task.service';

@Component({
  selector: 'app-selected-language',
  templateUrl: './selected-language.component.html',
  styleUrls: ['./selected-language.component.css']
})
export class SelectedLanguageComponent implements OnInit, OnDestroy {

  constructor(private taskService: TaskService) { }

  private displayMap: Map<string, string> = new Map<string, string>([
    ["csharp", "C#"],
    ["python", "Python"]
  ]);

  get displayLanguage() {
    return this.displayMap.get(this.selectedLanguage) || this.selectedLanguage;
  }

  selectedLanguage: string = '';

  private sub?: Subscription;

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(t => {
      this.selectedLanguage = t.Language;
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
