using SixLabors.ImageSharp.Formats.Jpeg;

namespace DespesasCartao
{
    public class AppUtils
    {
        public static async Task ProcessarArquivoDeImagem(int idProduto, IFormFile imagemPoduto, IWebHostEnvironment env)
        {   
            //copia a imagem para um stream em memoria
            var ms = new MemoryStream();
            await imagemPoduto.CopyToAsync(ms);
            //carrega o stream em memoria para o objeto de processamento de imagem
            ms.Position = 0;
            var img = await Image.LoadAsync(ms);
            JpegEncoder jpegEnc = new JpegEncoder
            {
                Quality = 100
            };
            //jpegEnc.Quality = 100;
            img.SaveAsJpeg(ms,jpegEnc);
            ms.Position = 0;
            img = await Image.LoadAsync(ms);
            ms.Close();
            ms.Dispose();

            //cria um retangulo de recorte para deixar a imagem quadrada
            var tamanho = img.Size;
            Rectangle retanguloCorte;
            if (tamanho.Width > tamanho.Height) 
            {
                float x = (tamanho.Width - tamanho.Height) / 2.0f;
                retanguloCorte = new Rectangle((int)x,0, tamanho.Height,tamanho.Height);
            }
            else
            {
                float y = (tamanho.Height - tamanho.Width) / 2.0f;
                retanguloCorte = new Rectangle((int)y, 0, tamanho.Width, tamanho.Width);
            }

            //recorta a imagem usando o retangulo computado
            img.Mutate(i => i.Crop(retanguloCorte));
            //monta o caminho da imagem(~/img/produto/000000.jpg)
            var caminhaArquivoImagem = Path.Combine(env.WebRootPath, "img\\produto", idProduto.ToString("D6") + ".jpg");
            //cria um arquivo de imagem subescrevendo o existente, caso exista
            await img.SaveAsync(caminhaArquivoImagem);
        }
    }
}
