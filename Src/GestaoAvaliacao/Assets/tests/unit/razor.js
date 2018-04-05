	
var settingsGeneral = [{"Key":"JPEG","Value":"True"},{"Key":"GIF","Value":"True"},{"Key":"PNG","Value":"True"},{"Key":"BMP","Value":"True"},{"Key":"IMAGE_GIF_COMPRESSION","Value":"False"},{"Key":"IMAGE_MAX_SIZE_FILE","Value":"3072"},{"Key":"IMAGE_QUALITY","Value":"100"},{"Key":"IMAGE_MAX_RESOLUTION_HEIGHT","Value":"200"},{"Key":"IMAGE_MAX_RESOLUTION_WIDTH","Value":"200"},{"Key":"UTILIZACDNMATHJAX","Value":"True"},{"Key":"FILE_MAX_SIZE","Value":"3072"},{"Key":"URL_API","Value":"http://localhost:8080"},{"Key":"CHAR_SEP_CSV","Value":";"}]
var settingsItem = [{"Key":"BASETEXT","Value":"Texto base","Obligatory":false,"Versioning":true},{"Key":"ITEMSITUATION","Value":"Situação do item","Obligatory":null,"Versioning":null},{"Key":"SOURCE","Value":"Fonte","Obligatory":false,"Versioning":true},{"Key":"DESCRIPTORSENTENCE","Value":"Sentença descritora do item","Obligatory":true,"Versioning":true},{"Key":"ITEMTYPE","Value":"Tipo do item","Obligatory":null,"Versioning":true},{"Key":"ITEMCURRICULUMGRADE","Value":"Selecione o(s) ano(s)","Obligatory":null,"Versioning":true},{"Key":"KEYWORDS","Value":"Palavras-chave","Obligatory":true,"Versioning":true},{"Key":"PROFICIENCY","Value":"Proficiência","Obligatory":true,"Versioning":true},{"Key":"ITEMLEVEL","Value":"Dificuldade sugerida","Obligatory":true,"Versioning":true},{"Key":"STATEMENT","Value":"Enunciado","Obligatory":true,"Versioning":true},{"Key":"TRI","Value":"TRI","Obligatory":false,"Versioning":true},{"Key":"TCT","Value":"TCT","Obligatory":false,"Versioning":true},{"Key":"TIPS","Value":"Observação","Obligatory":true,"Versioning":true},{"Key":"ALTERNATIVES","Value":"Alternativas","Obligatory":true,"Versioning":true},{"Key":"JUSTIFICATIVE","Value":"Justificativa","Obligatory":true,"Versioning":true},{"Key":"ISRESTRICT","Value":"Sigiloso","Obligatory":null,"Versioning":true},{"Key":"NIVEISMATRIZ","Value":"Alteração nos níveis da matriz","Obligatory":null,"Versioning":true},{"Key":"INITIAL_ORIENTATION","Value":"Orientação inicial para aplicador","Obligatory":false,"Versioning":true},{"Key":"INITIAL_STATEMENT","Value":"Enunciado de abertura do item","Obligatory":false,"Versioning":true},{"Key":"BASETEXT_ORIENTATION","Value":"Orientação complementar sobre o texto base","Obligatory":false,"Versioning":true}]
var settingsTest = [{"Key":"BASE_TEXT_DEFAULT","Value":"Utilize o texto a seguir para responder as questões"},{"Key":"CODE_ALTERNATIVE_DUPLICATE","Value":"RA"},{"Key":"CODE_ALTERNATIVE_EMPTY","Value":"NU"},{"Key":"STUDENT_NUMBER_ID","Value":"True"}]
var Parameters = {};
var jsontoParse = [];
jQuery.grep(settingsGeneral, function (s,i) { jsontoParse.push('\\"' +s.Key + '\\" : \\"' + s.Value + '\\"')});
//jQuery.grep(settings, function (s,i) { jsontoParse.push(s.Key + ' : "' + s.Value + '"')});
Parameters.General = JSON.parse(('{' + jsontoParse.join(',') + '}').replace(/\\/g, ''));

jsontoParse = [];
jQuery.grep(settingsItem, function (s,i) { jsontoParse.push('\\"' +s.Key + '\\" : { \\"Obligatory\\" : \\"' + s.Obligatory + '\\", \\"Value\\" : \\"' + s.Value + '\\", \\"Versioning\\" : \\"' + s.Versioning + '\\"  }')});

Parameters.Item = JSON.parse(('{' + jsontoParse.join(',') + '}').replace(/\\/g, ''));

jsontoParse = [];
jQuery.grep(settingsTest, function (s,i) { jsontoParse.push('\\"' +s.Key + '\\" : \\"' + s.Value + '\\"')});

Parameters.Test = JSON.parse(('{' + jsontoParse.join(',') + '}').replace(/\\/g, ''));



function base_url(url) {
    url = url || "";
    return 'http://localhost:2549/' + url;
};
	
function base_area_url(url) {
    url = url || "";
    return 'http://localhost:2549//' + url;
};
	
function template_url(directive, area, folder) {
    folder = folder ? folder + '/' : '';
    var path = folder + directive + '/' + directive + '.htm';

    if (area) {
        return base_url('areas//assets/js/directives/' + path);
    }
    else {
        return base_url('assets/js/angular/directives/_bundle/' + path);
    }
};

function getIsRestrict() {
    return 'True';
};

function api_url(service) {
    service = service || "";
    return 'http://localhost:50671/api/' + service;
};