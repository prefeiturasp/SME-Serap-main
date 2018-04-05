/**
 * function Plugin para inserir formulas pré definidas no Redactor
 * @namespace Controller
 * @author Julio Silva - 18/11/2015
 */
if (!RedactorPlugins) var RedactorPlugins = {};


RedactorPlugins.mathLatex = function () {
    return {
        init: function () {


            this.mathLatex.function_keys = [
                ['fração', 'function_1'],
                ['Potência', 'function_2'],
                ['Int', 'function_3'],
                ['Bigcap', 'function_4'],
                ['Soma', 'function_5'],
                ['Produto', 'function_6'],
                ['Subscript', 'function_7'],
                ['Tiny Fraction', 'function_8'],
                ['Int Interval', 'function_9'],
                ['Bigcap Interval', 'function_10'],
                ['Soma Interval', 'function_11'],
                ['Produto Interval', 'function_12'],
                ['Script Fraction', 'function_13'],
                ['Fraction Partial', 'function_14'],
                ['Int Oint', 'function_15'],
                ['Bigcup', 'function_16'],
                ['Raiz', 'function_17'],
                ['Coprod', 'function_18'],
                ['Script internal Potencia', 'function_19'],
                ['Fraction Partial two', 'function_20'],
                ['Int Oint Two', 'function_21'],
                ['Bigcup', 'function_22'],
                ['Raiz Two', 'function_23'],
                ['Coprod', 'function_24'],
                ['Texttrm', 'function_25'],
                ['Mathrm', 'function_26'],
                ['Iint', 'function_27'],
                ['Limite', 'function_28']
            ];

            this.mathLatex.bracket_keys = [
                ['()', 'bracket_1'],
                ['double_pipe', 'bracket_2'],
                ['colchete', 'bracket_3'],
                ['rangle', 'bracket_4'],
                ['chaves', 'bracket_5'],
                ['flor', 'bracket_6'],
                ['pipe', 'bracket_7'],
                ['flor inverse', 'bracket_8'],
                ['chave left', 'bracket_9'],
                ['chave right', 'bracket_10']
            ];

            this.mathLatex.greeklower_keys = [
                ['alpha', 'greeklower_1'],
                ['beta', 'greeklower_2'],
                ['gamma', 'greeklower_3'],
                ['delta', 'greeklower_4'],
                ['epsilon', 'greeklower_5'],
                ['varepsilon', 'greeklower_6'],
                ['zeta', 'greeklower_7'],
                ['eta', 'greeklower_8'],
                ['theta', 'greeklower_9'],
                ['vartheta', 'greeklower_10'],
                ['iota', 'greeklower_11'],
                ['kappa', 'greeklower_12'],
                ['lambda', 'greeklower_13'],
                ['mu', 'greeklower_14'],
                ['nu', 'greeklower_15'],
                ['xi', 'greeklower_16'],
                ['pi', 'greeklower_17'],
                ['varpi', 'greeklower_18'],
                ['rho', 'greeklower_19'],
                ['varrho', 'greeklower_20'],
                ['sigma', 'greeklower_21'],
                ['varsigma', 'greeklower_22'],
                ['tau', 'greeklower_23'],
                ['upsilon', 'greeklower_24'],
                ['phi', 'greeklower_25'],
                ['varphi', 'greeklower_26'],
                ['chi', 'greeklower_27'],
                ['psi', 'greeklower_28'],
                ['omega', 'greeklower_29'],
            ];

            this.mathLatex.greekupper_keys = [
               ['Gamma', 'greekupper_1'],
               ['Delta', 'greekupper_2'],
               ['Theta', 'greekupper_3'],
               ['Lambda', 'greekupper_4'],
               ['Xi', 'greekupper_5'],
               ['Pi', 'greekupper_6'],
               ['Sigma', 'greekupper_7'],
               ['Upsilon', 'greekupper_8'],
               ['Phi', 'greekupper_9'],
               ['Psi', 'greekupper_10'],
               ['Omega', 'greekupper_11']
            ];

            this.mathLatex.relations_keys = [
                ['relations_1', 'relations_1'],
                ['relations_2', 'relations_2'],
                ['relations_3', 'relations_3'],
                ['relations_4', 'relations_4'],
                ['relations_5', 'relations_5'],
                ['relations_6', 'relations_6'],
                ['relations_7', 'relations_7'],
                ['relations_8', 'relations_8'],
                ['relations_9', 'relations_9'],
                ['relations_10', 'relations_10'],
                ['relations_11', 'relations_11'],
                ['relations_12', 'relations_12'],
                ['relations_13', 'relations_13'],
                ['relations_14', 'relations_14'],
                ['relations_15', 'relations_15'],
                ['relations_16', 'relations_16'],
                ['relations_17', 'relations_17'],
                ['relations_18', 'relations_18'],
                ['relations_19', 'relations_19'],
                ['relations_20', 'relations_20'],
                ['relations_21', 'relations_21'],
                ['relations_22', 'relations_22'],
                ['relations_23', 'relations_23'],
                ['relations_24', 'relations_24'],
                ['relations_25', 'relations_25'],
                ['relations_26', 'relations_26'],
                ['relations_27', 'relations_27'],
                ['relations_28', 'relations_28'],
                ['relations_29', 'relations_29'],
                ['relations_30', 'relations_30'],
                ['relations_31', 'relations_31'],
                ['relations_32', 'relations_32'],
                ['relations_33', 'relations_33'],
                ['relations_34', 'relations_34'],
                ['relations_35', 'relations_35'],
                ['relations_36', 'relations_36'],
                ['relations_37', 'relations_37'],
                ['relations_38', 'relations_38']
            ];

            this.mathLatex.arrow_keys = [
                ['arrow_1', 'arrow_1'],
                ['arrow_1', 'arrow_2'],
                ['arrow_1', 'arrow_3'],
                ['arrow_1', 'arrow_4'],
                ['arrow_1', 'arrow_5'],
                ['arrow_1', 'arrow_6'],
                ['arrow_1', 'arrow_7'],
                ['arrow_1', 'arrow_8'],
                ['arrow_1', 'arrow_9'],
                ['arrow_1', 'arrow_10'],
                ['arrow_1', 'arrow_11'],
                ['arrow_1', 'arrow_12'],
                ['arrow_1', 'arrow_13'],
                ['arrow_1', 'arrow_14'],
                ['arrow_1', 'arrow_15'],
                ['arrow_1', 'arrow_16'],
                ['arrow_1', 'arrow_17'],
                ['arrow_1', 'arrow_18'],
                ['arrow_1', 'arrow_19'],
                ['arrow_1', 'arrow_20']
            ];

            this.mathLatex.accents_ext_keys = [
                ['accents_ext_1', 'accents_ext_1'],
                ['accents_ext_2', 'accents_ext_2'],
                ['accents_ext_3', 'accents_ext_3'],
                ['accents_ext_4', 'accents_ext_4'],
                ['accents_ext_5', 'accents_ext_5'],
                ['accents_ext_6', 'accents_ext_6'],
                ['accents_ext_7', 'accents_ext_7'],
                ['accents_ext_8', 'accents_ext_8'],
                ['accents_ext_9', 'accents_ext_9'],
                ['accents_ext_10', 'accents_ext_10'],
            ];

            this.mathLatex.accents_keys = [
                ['accents_1', 'accents_1'],
                ['accents_2', 'accents_2'],
                ['accents_3', 'accents_3'],
                ['accents_4', 'accents_4'],
                ['accents_5', 'accents_5'],
                ['accents_6', 'accents_6'],
                ['accents_7', 'accents_7'],
                ['accents_8', 'accents_8'],
                ['accents_9', 'accents_9'],
                ['accents_10', 'accents_10'],
                ['accents_12', 'accents_11'],
                ['accents_13', 'accents_12'],
                ['accents_14', 'accents_13'],
                ['accents_15', 'accents_14']
            ];

            this.mathLatex.subsupset_keys = [
                 ['subsupset_1', 'subsupset_1'],
                 ['subsupset_2', 'subsupset_2'],
                 ['subsupset_3', 'subsupset_3'],
                 ['subsupset_4', 'subsupset_4'],
                 ['subsupset_5', 'subsupset_5'],
                 ['subsupset_6', 'subsupset_6'],
                 ['subsupset_7', 'subsupset_7'],
                 ['subsupset_8', 'subsupset_8'],
                 ['subsupset_9', 'subsupset_9'],
                 ['subsupset_10', 'subsupset_10'],
                 ['subsupset_11', 'subsupset_11'],
                 ['subsupset_12', 'subsupset_12'],
                 ['subsupset_13', 'subsupset_13'],
                 ['subsupset_14', 'subsupset_14'],
                 ['subsupset_15', 'subsupset_15'],
                 ['subsupset_16', 'subsupset_16'],
                 ['subsupset_17', 'subsupset_17'],
            ];

            this.mathLatex.symbols_keys = [
                 ['symbols_1', 'symbols_1'],
                 ['symbols_2', 'symbols_2'],
                 ['symbols_3', 'symbols_3'],
                 ['symbols_4', 'symbols_4'],
                 ['symbols_5', 'symbols_5'],
                 ['symbols_6', 'symbols_6'],
                 ['symbols_7', 'symbols_7'],
                 ['symbols_8', 'symbols_8'],
                 ['symbols_9', 'symbols_9'],
                 ['symbols_10', 'symbols_10'],
                 ['symbols_11', 'symbols_11'],
                 ['symbols_12', 'symbols_12'],
                 ['symbols_13', 'symbols_13'],
                 ['symbols_14', 'symbols_14'],
                 ['symbols_15', 'symbols_15'],
                 ['symbols_16', 'symbols_16'],
                 ['symbols_17', 'symbols_17'],
                 ['symbols_18', 'symbols_18'],
                 ['symbols_19', 'symbols_19'],
                 ['symbols_20', 'symbols_20'],
                 ['symbols_21', 'symbols_21'],
                 ['symbols_22', 'symbols_22'],
                 ['symbols_23', 'symbols_23'],
                 ['symbols_24', 'symbols_24'],
                 ['symbols_26', 'symbols_26'],
                 ['symbols_27', 'symbols_27'],
                 ['symbols_28', 'symbols_28'],
                 ['symbols_30', 'symbols_30'],
            ];

            this.mathLatex.matrix_keys = [
                ['matrix_1', 'matrix_1'],
                ['matrix_2', 'matrix_2'],
                ['matrix_3', 'matrix_3'],
                ['matrix_4', 'matrix_4'],
                ['matrix_5', 'matrix_5'],
                ['matrix_6', 'matrix_6'],
                ['matrix_7', 'matrix_7'],
                ['matrix_8', 'matrix_8'],
                ['matrix_9', 'matrix_9'],
                ['matrix_10', 'matrix_10'],
                ['matrix_11', 'matrix_11'],
                ['matrix_12', 'matrix_12'],
                ['matrix_13', 'matrix_13']
            ];


            this.mathLatex.function_dictionary = {
                function_1: 'x^{a}',
                function_2: '\\frac{a}{b}',
                function_3: '\\int',
                function_4: '\\bigcap',
                function_5: '\\sum',
                function_6: '\\prod',
                function_7: 'x_{a}',
                function_8: 'x\\tfrac{a}{b}',
                function_9: '\\int_{a}^{b}',
                function_10: '\\bigcap_{a}^{b}',
                function_11: '\\sum_{a}^{b}',
                function_12: '\\prod_{a}^{b}',
                function_13: 'x_{a}^{b}',
                function_14: '\\frac{\\partial }{\\partial x}',
                function_15: '\\oint',
                function_16: '\\bigcup',
                function_17: '\\sqrt{x}',
                function_18: '\\coprod',
                function_19: '{x_{a}}^{b}',
                function_20: '\\frac{\\partial^2 }{\\partial x^2}',
                function_21: '\\oint_{a}^{b}',
                function_22: '\\bigcup_{b}^{a}',
                function_23: '\\sqrt[n]{x}',
                function_24: '\\coprod_{a}^{b}',
                function_25: '_{b}^{a}\\textrm{C}',
                function_26: '\\frac{\\mathrm{d} }{\\mathrm{d} x}',
                function_27: '\\iint_{b}^{a}',
                function_28: '\\lim_{x\\to0}'
            };

            this.mathLatex.bracket_dictionary = {
                bracket_1: '\\left ( \\right )',
                bracket_2: '\\left \\|  \\right \\|',
                bracket_3: '\\left [  \\right ]',
                bracket_4: '\\left \\langle  \\right \\rangle',
                bracket_5: '\\left \\{  \\right \\}',
                bracket_6: '\\left \\lfloor  \\right \\rfloor',
                bracket_7: '\\left |  \\right |',
                bracket_8: '\\left \\lceil  \\right \\rceil',
                bracket_9: '\\left \\{  \\right.',
                bracket_10: '\\left.  \\right \\}'
            };

            this.mathLatex.greeklower_dictionary = {
                greeklower_1: '\\alpha',
                greeklower_2: '\\beta',
                greeklower_3: '\\gamma',
                greeklower_4: '\\delta',
                greeklower_5: '\\epsilon',
                greeklower_6: '\\varepsilon',
                greeklower_7: '\\zeta',
                greeklower_8: '\\eta',
                greeklower_9: '\\theta',
                greeklower_10: '\\vartheta',
                greeklower_11: '\\iota',
                greeklower_12: '\\kappa',
                greeklower_13: '\\lambda',
                greeklower_14: '\\mu',
                greeklower_15: '\\nu',
                greeklower_16: '\\xi',
                greeklower_17: '\\pi',
                greeklower_18: '\\varpi',
                greeklower_19: '\\rho',
                greeklower_20: '\\varrho',
                greeklower_21: '\\sigma',
                greeklower_22: '\\varsigma',
                greeklower_23: '\\tau',
                greeklower_24: '\\upsilon',
                greeklower_25: '\\phi',
                greeklower_26: '\\varphi',
                greeklower_27: '\\chi',
                greeklower_28: '\\psi',
                greeklower_29: '\\omega'
            };

            this.mathLatex.greekupper_dictionary = {
                greekupper_1: '\\Gamma',
                greekupper_2: '\\Delta',
                greekupper_3: '\\Theta',
                greekupper_4: '\\Lambda ',
                greekupper_5: '\\Xi ',
                greekupper_6: '\\Pi',
                greekupper_7: '\\Sigma ',
                greekupper_8: '\\Upsilon ',
                greekupper_9: '\\Phi ',
                greekupper_10: '\\Psi ',
                greekupper_11: '\\Omega '
            };

            this.mathLatex.relations_dictionary = {
                relations_1: '<',
                relations_2: '>',
                relations_3: '=',
                relations_4: '\\leq',
                relations_5: '\\geq',
                relations_6: '\\doteq',
                relations_7: '\\leqslant',
                relations_8: '\\geqslant',
                relations_9: '\\equiv',
                relations_10: '\\nless',
                relations_11: '\\ngtr',
                relations_12: '\\neq',
                relations_13: '\\nleqslant',
                relations_14: '\\ngeqslant',
                relations_15: '\\not\\equiv',
                relations_16: '\\prec',
                relations_17: '\\succ',
                relations_18: '=',
                relations_19: '\\preceq',
                relations_20: '\\succeq',
                relations_21: '\\sim',
                relations_22: '\\ll',
                relations_23: '\\gg',
                relations_24: '\\approx',
                relations_25: '\\vdash',
                relations_26: '\\dashv',
                relations_27: '\\simeq',
                relations_28: '\\smile',
                relations_29: '\\frown',
                relations_30: '\\cong',
                relations_31: '\\models',
                relations_32: '\\perp',
                relations_33: '\\asymp',
                relations_34: '\\mid',
                relations_35: '\\parallel',
                relations_36: '\\propto',
                relations_37: '\\bowtie',
                relations_38: '\\Join'
            };

            this.mathLatex.arrow_dictionary = {
                arrow_1: 'x \\mapsto x^2',
                arrow_2: 'n \\to',
                arrow_3: '\\leftarrow',
                arrow_4: '\\rightarrow',
                arrow_5: '\\Leftarrow',
                arrow_6: '\\Rightarrow',
                arrow_7: '\\leftrightarrow',
                arrow_8: '\\Leftrightarrow ',
                arrow_9: '\\leftharpoonup',
                arrow_10: '\\rightharpoonup',
                arrow_11: '\\leftharpoondown',
                arrow_12: '\\rightharpoondown',
                arrow_13: '\\leftrightharpoons',
                arrow_14: '\\rightleftharpoons',
                arrow_15: '\\xleftarrow[text]{1}',
                arrow_16: '\\xrightarrow[text]{1}',
                arrow_17: '\\overset{a}{\\leftarrow}',
                arrow_18: '\\overset{a}{\\rightarrow}',
                arrow_19: '\\underset{a}{\\leftarrow}',
                arrow_20: '\\underset{a}{\\rightarrow}',
            };

            this.mathLatex.accents_ext_dictionary = {
                accents_ext_1: '\\widetilde{abc}',
                accents_ext_2: '\\widehat{abc}',
                accents_ext_3: '\\overleftarrow{abc}',
                accents_ext_4: '\\overrightarrow{abc}',
                accents_ext_5: '\\overline{abc}',
                accents_ext_6: '\\underline{abc}',
                accents_ext_7: '\\overbrace{abc}',
                accents_ext_8: '\\underbrace{abc}',
                accents_ext_9: '\\overset{a}{abc}',
                accents_ext_10: '\\underset{abc}{a}',
            };

            this.mathLatex.accents_dictionary = {
                accents_1: '{a}\'',
                accents_2: '{a}\'\'',
                accents_3: '\\dot{a}',
                accents_4: '\\ddot{a}',
                accents_5: '\\hat{a}',
                accents_6: '\\check{a}',
                accents_7: '\\grave{a}',
                accents_8: '\\acute{a}',
                accents_9: '\\tilde{a}',
                accents_10: '\\breve{a} ',
                accents_11: '\\bar{a}',
                accents_12: '\\vec{a}',
                accents_13: '\\not{a}',
                accents_14: 'a^{\\circ}',
            };

            this.mathLatex.subsupset_dictionary = {
                subsupset_1: '\\sqsubset',
                subsupset_2: '\\sqsupset',
                subsupset_3: '\\sqsubseteq',
                subsupset_4: '\\sqsupseteq',
                subsupset_5: '\\subset',
                subsupset_6: '\\supset',
                subsupset_7: '\\subseteq',
                subsupset_8: '\\supseteq',
                subsupset_9: '\\nsubseteq',
                subsupset_10: '\\nsupseteq',
                subsupset_11: '\\subseteqq',
                subsupset_12: '\\supseteqq',
                subsupset_13: '\\nsubseteqq',
                subsupset_14: '\\nsupseteqq',
                subsupset_15: '\\in',
                subsupset_16: '\\ni',
                subsupset_17: '\\notin',
            };

            this.mathLatex.symbols_dictionary = {
                symbols_1: '\\therefore',
                symbols_2: '\\partial',
                symbols_3: '\\mathbb{P}',
                symbols_4: '\\angle',
                symbols_5: '\\because',
                symbols_6: '\\imath',
                symbols_7: '\\mathbb{N}',
                symbols_8: '\\measuredangle',
                symbols_9: '\\cdots ',
                symbols_10: '\\jmath',
                symbols_11: '\\mathbb{Z}',
                symbols_12: '\\sphericalangle',
                symbols_13: '\\ddots',
                symbols_14: '\\Re',
                symbols_15: '\\mathbb{I}',
                symbols_16: '\\varnothing',
                symbols_17: '\\vdots',
                symbols_18: '\\Im',
                symbols_19: '\\mathbb{Q}',
                symbols_20: '\\infty',
                symbols_21: '\\S',
                symbols_22: '\\forall',
                symbols_23: '\\mathbb{R}',
                symbols_24: '\\mho',
                symbols_26: '\\exists',
                symbols_27: '\\mathbb{C} ',
                symbols_28: '\\wp ',
                symbols_30: '\\top',
            };

            this.mathLatex.matrix_dictionary = {
                matrix_1: '\\begin{matrix}1&1&1\\\\1&1&1\\end{matrix}',
                matrix_2: '\\begin{bmatrix}1&1&1\\\\1&1&1\\\\1&1&1\\end{bmatrix}',
                matrix_3: '\\binom{r}{n}',
                matrix_4: '\\begin{pmatrix}1&1&1\\\\1&1&1\\\\1&1&1\\end{pmatrix}',
                matrix_5: '\\bigl(\\begin{smallmatrix}1&1&1\\\\1&1&1\\\\1&1&1\\end{smallmatrix}\\bigr)',
                matrix_6: '\\begin{cases}&\\\\text{ if } x= 1\\\\&\\\\text{ if } x= 1\\end{cases}',
                matrix_7: '\\begin{vmatrix}1&1&1\\\\1&1&1\\\\1&1&1\\end{vmatrix}',
                matrix_8: '\\begin{Bmatrix}1&1&1\\\\1&1&1\\\\1&1&1\\end{Bmatrix}',
                matrix_9: '\\begin{align*}x &= 1\\\\y &= 2\\end{align*}',
                matrix_10: '\\begin{Vmatrix}1&1&1\\\\1&1&1\\\\1&1&1\\end{Vmatrix}',
                matrix_11: '\\left\\{\\begin{matrix}1&1\\\\1&1\\end{matrix}\\right.',
                matrix_12: '\\left.\\begin{matrix}1&1&\\\\1&1&\\end{matrix}\\right|',
                matrix_13: '\\left.\\begin{matrix}1&1\\\\1&1\\end{matrix}\\right\\}'
            };


            //Editor Matemático
            this.mathLatex.mathEditor = $('<div id="editor-equations" class="mathEditor row"></div>');


            //Functions
            var functions = $('<div id="functions" class="col-md-12 equation-space"></div>');
            var ul_functions = $('<ul class="redactor-modal-eq">');
            for (var i = 0; i < this.mathLatex.function_keys.length; i++) {
                var li = $('<li class="redactor-eq-link">').addClass(this.mathLatex.function_keys[i][1]);
                li.attr('key', this.mathLatex.function_keys[i][1]);
                li.attr('dictionary_type', 'function');
                li.attr('data-trigger', 'hover');
                li.attr('data-type', 'success');
                li.attr('data-title', this.mathLatex.function_keys[i][0]);
                li.attr('data-placement', 'top');
                li.attr('data-animation', 'am-fade');
                li.attr('data-container', 'body');
                li.attr('bs-tooltip', '');
                ul_functions.append(li);
            }
            functions.append(ul_functions);

            //Brakets
            var bracket = $('<div id="braket" class="col-md-12 equation-space"></div>');
            var ul_bracket = $('<ul class="redactor-modal-eq">');
            for (var a = 0; a < this.mathLatex.bracket_keys.length; a++) {
                var li = $('<li class="redactor-eq-link">').addClass(this.mathLatex.bracket_keys[a][1]);
                li.attr('key', this.mathLatex.bracket_keys[a][1]);
                li.attr('dictionary_type', 'bracket');
                li.attr('data-trigger', 'hover');
                li.attr('data-type', 'success');
                li.attr('data-title', this.mathLatex.bracket_keys[a][0]);
                li.attr('data-placement', 'top');
                li.attr('data-animation', 'am-fade');
                li.attr('data-container', 'body');
                li.attr('bs-tooltip', '');
                ul_bracket.append(li);
            }
            bracket.append(ul_bracket);

            //Greeklower
            var greeklower = $('<div id="greeklower" class="col-md-12 equation-space"></div>');
            var ul_greeklower = $('<ul class="redactor-modal-eq">');
            for (var b = 0; b < this.mathLatex.greeklower_keys.length; b++) {
                var li = $('<li class="redactor-eq-link">').addClass(this.mathLatex.greeklower_keys[b][1]);
                li.attr('key', this.mathLatex.greeklower_keys[b][1]);
                li.attr('dictionary_type', 'greeklower');
                li.attr('data-trigger', 'hover');
                li.attr('data-type', 'success');
                li.attr('data-title', this.mathLatex.greeklower_keys[b][0]);
                li.attr('data-placement', 'top');
                li.attr('data-animation', 'am-fade');
                li.attr('data-container', 'body');
                li.attr('bs-tooltip', '');
                ul_greeklower.append(li);
            }
            greeklower.append(ul_greeklower);


            //Greekupper
            var greekupper = $('<div id="greeklower" class="col-md-12 equation-space"></div>');
            var ul_greekupper = $('<ul class="redactor-modal-eq">');
            for (var c = 0; c < this.mathLatex.greekupper_keys.length; c++) {
                var li = $('<li class="redactor-eq-link">').addClass(this.mathLatex.greekupper_keys[c][1]);
                li.attr('key', this.mathLatex.greekupper_keys[c][1]);
                li.attr('dictionary_type', 'greekupper');
                li.attr('data-trigger', 'hover');
                li.attr('data-type', 'success');
                li.attr('data-title', this.mathLatex.greekupper_keys[c][0]);
                li.attr('data-placement', 'top');
                li.attr('data-animation', 'am-fade');
                li.attr('data-container', 'body');
                li.attr('bs-tooltip', '');
                ul_greekupper.append(li);
            }
            greekupper.append(ul_greekupper);


            //Relations
            var relations = $('<div id="relations" class="col-md-12 equation-space"></div>');
            var ul_relations = $('<ul class="redactor-modal-eq">');
            for (var d = 0; d < this.mathLatex.relations_keys.length; d++) {
                var li = $('<li class="redactor-eq-link">').addClass(this.mathLatex.relations_keys[d][1]);
                li.attr('key', this.mathLatex.relations_keys[d][1]);
                li.attr('dictionary_type', 'relations');
                li.attr('data-trigger', 'hover');
                li.attr('data-type', 'success');
                li.attr('data-title', this.mathLatex.relations_keys[d][0]);
                li.attr('data-placement', 'top');
                li.attr('data-animation', 'am-fade');
                li.attr('data-container', 'body');
                li.attr('bs-tooltip', '');
                ul_relations.append(li);
            }
            relations.append(ul_relations);


            //Arrows
            var arrows = $('<div id="arrows" class="col-md-12 equation-space"></div>');
            var ul_arrows = $('<ul class="redactor-modal-eq">');
            for (var e = 0; e < this.mathLatex.arrow_keys.length; e++) {
                var li = $('<li class="redactor-eq-link">').addClass(this.mathLatex.arrow_keys[e][1]);
                li.attr('key', this.mathLatex.arrow_keys[e][1]);
                li.attr('dictionary_type', 'arrow');
                li.attr('data-trigger', 'hover');
                li.attr('data-type', 'success');
                li.attr('data-title', this.mathLatex.arrow_keys[e][0]);
                li.attr('data-placement', 'top');
                li.attr('data-animation', 'am-fade');
                li.attr('data-container', 'body');
                li.attr('bs-tooltip', '');
                ul_arrows.append(li);
            }
            arrows.append(ul_arrows);


            //Accents_ext
            var accents_ext = $('<div id="accents_ext" class="col-md-12 equation-space"></div>');
            var ul_accents_ext = $('<ul class="redactor-modal-eq">');
            for (var f = 0; f < this.mathLatex.accents_ext_keys.length; f++) {
                var li = $('<li class="redactor-eq-link">').addClass(this.mathLatex.accents_ext_keys[f][1]);
                li.attr('key', this.mathLatex.accents_ext_keys[f][1]);
                li.attr('dictionary_type', 'accents_ext');
                li.attr('data-trigger', 'hover');
                li.attr('data-type', 'success');
                li.attr('data-title', this.mathLatex.accents_ext_keys[f][0]);
                li.attr('data-placement', 'top');
                li.attr('data-animation', 'am-fade');
                li.attr('data-container', 'body');
                li.attr('bs-tooltip', '');
                ul_accents_ext.append(li);
            }
            accents_ext.append(ul_accents_ext);


            //Accents
            var accents = $('<div id="accents" class="col-md-12 equation-space"></div>');
            var ul_accents = $('<ul class="redactor-modal-eq">');
            for (var g = 0; g < this.mathLatex.accents_keys.length; g++) {
                var li = $('<li class="redactor-eq-link">').addClass(this.mathLatex.accents_keys[g][1]);
                li.attr('key', this.mathLatex.accents_keys[g][1]);
                li.attr('dictionary_type', 'accents');
                li.attr('data-trigger', 'hover');
                li.attr('data-type', 'success');
                li.attr('data-title', this.mathLatex.accents_keys[g][0]);
                li.attr('data-placement', 'top');
                li.attr('data-animation', 'am-fade');
                li.attr('data-container', 'body');
                li.attr('bs-tooltip', '');
                ul_accents.append(li);
            }
            accents.append(ul_accents);


            //Subsupset
            var subsupset = $('<div id="subsupset" class="col-md-12 equation-space"></div>');
            var ul_subsupset = $('<ul class="redactor-modal-eq">');
            for (var h = 0; h < this.mathLatex.subsupset_keys.length; h++) {
                var li = $('<li class="redactor-eq-link">').addClass(this.mathLatex.subsupset_keys[h][1]);
                li.attr('key', this.mathLatex.subsupset_keys[h][1]);
                li.attr('dictionary_type', 'subsupset');
                li.attr('data-trigger', 'hover');
                li.attr('data-type', 'success');
                li.attr('data-title', this.mathLatex.subsupset_keys[h][0]);
                li.attr('data-placement', 'top');
                li.attr('data-animation', 'am-fade');
                li.attr('data-container', 'body');
                li.attr('bs-tooltip', '');
                ul_subsupset.append(li);
            }
            subsupset.append(ul_subsupset);

            //Symbols
            var symbols = $('<div id="foreign" class="col-md-12 equation-space"></div>');
            var ul_symbols = $('<ul class="redactor-modal-eq">');
            for (var j = 0; j < this.mathLatex.symbols_keys.length; j++) {
                var li = $('<li class="redactor-eq-link">').addClass(this.mathLatex.symbols_keys[j][1]);
                li.attr('key', this.mathLatex.symbols_keys[j][1]);
                li.attr('dictionary_type', 'symbols');
                li.attr('data-trigger', 'hover');
                li.attr('data-type', 'success');
                li.attr('data-title', this.mathLatex.symbols_keys[j][0]);
                li.attr('data-placement', 'top');
                li.attr('data-animation', 'am-fade');
                li.attr('data-container', 'body');
                li.attr('bs-tooltip', '');
                ul_symbols.append(li);
            }
            symbols.append(ul_symbols);


            //Matrix
            var matrix = $('<div id="matrix" class="col-md-12 equation-space"></div>');
            var ul_matrix = $('<ul class="redactor-modal-eq">');
            for (var k = 0; k < this.mathLatex.matrix_keys.length; k++) {
                var li = $('<li class="redactor-eq-link">').addClass(this.mathLatex.matrix_keys[k][1]);
                li.attr('key', this.mathLatex.matrix_keys[k][1]);
                li.attr('dictionary_type', 'matrix');
                li.attr('data-trigger', 'hover');
                li.attr('data-type', 'success');
                li.attr('data-title', this.mathLatex.matrix_keys[k][0]);
                li.attr('data-placement', 'top');
                li.attr('data-animation', 'am-fade');
                li.attr('data-container', 'body');
                li.attr('bs-tooltip', '');
                ul_matrix.append(li);
            }
            matrix.append(ul_matrix);


            //Adicionar os grupos ao editor
            this.mathLatex.mathEditor.append(functions);
            this.mathLatex.mathEditor.append(bracket);
            this.mathLatex.mathEditor.append(greeklower);
            this.mathLatex.mathEditor.append(greekupper);
            this.mathLatex.mathEditor.append(relations);
            this.mathLatex.mathEditor.append(arrows);
            this.mathLatex.mathEditor.append(accents_ext);
            this.mathLatex.mathEditor.append(accents);
            this.mathLatex.mathEditor.append(subsupset);
            this.mathLatex.mathEditor.append(symbols);
            this.mathLatex.mathEditor.append(matrix);


            //Botão Toolbar Redactor
            var button = this.button.add('mathLatex', 'Fórmulas');
            var $button = this.button.get('mathLatex');
            $button.removeClass('redactor-btn-image').addClass('fa-redactor-btn');
            $button.html('<i class="material-icons" style="font-size: 1.4rem; vertical-align: initial;">functions</i>');
            this.button.addCallback(button, this.mathLatex.show);

        },

        show: function () {
            this.modal.addTemplate('mathLatex', '<section>' + this.utils.getOuterHtml(this.mathLatex.mathEditor) + '</section>');
            this.modal.load('mathLatex', 'Inserir Equação', 400);
            this.modal.createCancelButton();
            this.modal.show();
            $('.redactor-modal-eq').find('.redactor-eq-link').each($.proxy(this.mathLatex.load, this));
            this.selection.save();        
        },


        load: function (i, s) {
            $(s).on('click', $.proxy(function (e) {
                e.preventDefault();
                var key = $(s).attr('key');
                var dictionary_type = $(s).attr('dictionary_type');
                var latex = this.mathLatex[dictionary_type + '_dictionary'][key];
                if (key === 'equation') {
                    this.mathLatex.insertEquationSpan(latex);
                }
                else {
                    this.mathLatex.insertEquation(latex);
                }
            }, this));
        },


        insertEquationSpan: function (text) {
            this.selection.restore();
            var current = $(this.selection.getCurrent());
            if (current.hasClass('eq-redactor'))
                return;
            this.insert.html('<span class="eq-redactor">' + text + '</span>');
            this.modal.close();
            this.observe.load();
            this.selection.restore();
            this.code.sync();
        },
        insertEquation: function (text) {
            this.selection.restore();
            var current = $(this.selection.getCurrent());
            if (current.hasClass('eq-redactor')) {
                this.mathLatex.clearEquationSpan(current);
                this.insert.html(text);
                this.modal.close();
                this.observe.load();
                this.selection.restore();
                this.code.sync();
                this.mathLatex.clearEquationSpan(current);
            }
            else {
                this.insert.text(text);
                this.modal.close();
                this.observe.load();
                this.selection.restore();
                this.code.sync();
            }
        },
        clearEquationSpan: function (current) {
            current.find('.redactor-invisible-space').each(function (_i, _s) {
                $(_s).remove();
            });
        }
    };
};

