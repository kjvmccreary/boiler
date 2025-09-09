import type * as monaco from 'monaco-editor';
import {
  JSONLOGIC_OPERATORS,
  JSONLOGIC_OPERATOR_LABELS,
  getJsonLogicVariables
} from './jsonlogicOperators';

let registered = false;

export function registerJsonLogicEnhancements(m: typeof monaco) {
  if (registered) return;
  registered = true;

  // Operator completion (top-level)
  m.languages.registerCompletionItemProvider('json', {
    triggerCharacters: ['"', '{'],
    provideCompletionItems(model, position) {
      try {
        const text = model.getValue();
        const before = text.slice(0, model.getOffsetAt(position));
        const openBraces = (before.match(/{/g) || []).length;
        const closeBraces = (before.match(/}/g) || []).length;
        const depth = openBraces - closeBraces;
        if (depth > 1) return { suggestions: [] };

        const wordUntil = model.getWordUntilPosition(position);
        const range: monaco.IRange = {
          startLineNumber: position.lineNumber,
          endLineNumber: position.lineNumber,
          startColumn: wordUntil.startColumn,
          endColumn: wordUntil.endColumn
        };

        const suggestions: monaco.languages.CompletionItem[] = JSONLOGIC_OPERATORS.map(op => ({
          label: op.label,
          kind: m.languages.CompletionItemKind.Function,
          insertText: op.insertText,
          insertTextRules: m.languages.CompletionItemInsertTextRule.InsertAsSnippet,
          detail: op.detail,
          documentation: { value: op.documentation },
          range,
          sortText: op.label.padStart(3, '0')
        }));
        return { suggestions };
      } catch {
        return { suggestions: [] };
      }
    }
  });

  // Variable completion (context-aware inside "var" values OR anywhere to help users)
  m.languages.registerCompletionItemProvider('json', {
    triggerCharacters: ['"', '.'],
    provideCompletionItems(model, position) {
      const variables = getJsonLogicVariables();
      if (!variables.length) return { suggestions: [] };

      const word = model.getWordUntilPosition(position);
      const range: monaco.IRange = {
        startLineNumber: position.lineNumber,
        endLineNumber: position.lineNumber,
        startColumn: word.startColumn,
        endColumn: word.endColumn
      };

      const suggestions: monaco.languages.CompletionItem[] = variables.map(v => ({
        label: v,
        kind: m.languages.CompletionItemKind.Variable,
        insertText: v,
        detail: 'Context variable',
        documentation: { value: `**Variable**\n\nResolved at runtime: \`${v}\`` },
        range,
        sortText: 'zzz_' + v // appear after operators
      }));

      return { suggestions };
    }
  });

  // Hover provider (operators & variables)
  m.languages.registerHoverProvider('json', {
    provideHover(model, position) {
      const word = model.getWordAtPosition(position);
      if (!word) return null;

      if (JSONLOGIC_OPERATOR_LABELS.has(word.word)) {
        const meta = JSONLOGIC_OPERATORS.find(o => o.label === word.word);
        if (!meta) return null;
        return {
          contents: [
            { value: `### ${meta.label}` },
            { value: meta.documentation }
          ]
        };
      }

      const variables = getJsonLogicVariables();
      if (variables.includes(word.word)) {
        return {
          contents: [
            { value: `**Variable:** \`${word.word}\`` },
            { value: 'Provided by workflow runtime context or user input.' }
          ]
        };
      }
      return null;
    }
  });
}
