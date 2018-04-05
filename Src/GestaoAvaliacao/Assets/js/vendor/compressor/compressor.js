/**
 * Cria um objeto compressor.
 * @constructor
 */
var compressor = {

    /**
     * Recebe um objeto Image (JPG | PNG | GIF) e retorn um novo objeto Image comprimmido e redimensionado
     * @param {Image} source_img_obj imagem
     * @param {Integer} porcentagem de qualidade
     * @param {String}  extensão do item
     * @param {Integer} resolução máxima - largura
     * @param {Integer} resolução máxima - altura
     * @return {Image} retorna a imagem comprimida
     */

    compress: function ( source_img_obj, quality, output_format, maxWidth, maxHeight ) {

        // obter tipo da imagem
        var mime_type = "image/jpeg";
        if (typeof output_format !== "undefined" && output_format.toLocaleUpperCase() === "PNG") {
            mime_type = "image/png";
        }
        else if (typeof output_format !== "undefined" && output_format.toLocaleUpperCase() === "GIF") {
            mime_type = "image/gif";
        }

        // armazenar novas proporções
        var size = { height: 0, width: 0 };
        var ratio = Math.min(maxWidth / source_img_obj.naturalWidth, maxHeight / source_img_obj.naturalHeight);
        if ( source_img_obj.naturalWidth > maxWidth || source_img_obj.naturalHeight > maxHeight) {
            size.width = source_img_obj.naturalWidth * ratio;
            size.height = source_img_obj.naturalHeight*ratio;
        }
        else {
            size.width = source_img_obj.naturalWidth;
            size.height = source_img_obj.naturalHeight;
        }
           
        // criar um canvas e manipular a imagem
        var cvs = document.createElement('canvas');

        // injetar o tamanho da imagem no canvas
        cvs.width  = size.width;
        cvs.height = size.height;

        var ctx = cvs.getContext('2d');

        var draw = ctx.drawImage(source_img_obj, 0, 0, size.width, size.height);

        var newImageData = cvs.toDataURL(mime_type, quality / 100);

        var result_image_obj = new Image();

        result_image_obj.src = newImageData;

        return result_image_obj;
    }
    
};