using EjRecuperacionBUENO;
using Xunit;
using Assert = Xunit.Assert;


namespace Expendedora.Pruebas;

public class PruebasExpendedora
{
    [Xunit.Theory]
    [InlineData(Producto.Cafe, 90)]
    [InlineData(Producto.Chocolate, 110)]
    [InlineData(Producto.Cruasan, 160)]

    public void Precio_ProductoValido_DevuelveCentimos(
        Producto producto, int esperado)
    {
        int resultado = Program.Precio(producto);
        
        Assert.Equal(esperado, resultado);  
    }

    [Xunit.Theory]
    [InlineData(10, true)]
    [InlineData(20, true)]
    [InlineData(50, true)]
    [InlineData(100, true)]
    [InlineData(200, true)]
    [InlineData(0, false)]
    [InlineData(-10, false)]
    [InlineData(30, false)]
    [InlineData(500, false)]
    public void EsMonedaValida_CompruebaValor(
        int centimos, bool esperado)
    {
        bool resultado = Program.EsMonedaValida(centimos);
        
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void Precio_ProductoInexistente_LanzaExcepcion()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Program.Precio((Producto)99));
    }

    [Xunit.Theory]
    [InlineData(90, 90, 0)]
    [InlineData(100, 90, 10)]
    [InlineData(200, 110, 90)]
    public void Cambio_PagadoSuficiente_DevuelveDiferencia(
        int pagado, int precio, int esperado)
    {
        int resultado = Program.Cambio(pagado, precio);
        
        Assert.Equal(esperado, resultado);
    }
    
    [Fact]
    public void Cambio_PagoInsuficiente_LanzaExcepcion()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Program.Cambio(80, 90));
    }

    [Xunit.Theory]
    [InlineData(0, "Sin cambio")]
    [InlineData(10, "1 x 0,10 € ")]
    [InlineData(90, "1 x 0,50 €  · 2 x 0,20 € ")]
    [InlineData(380,
        "1 x 2,00 €  · 1 x 1,00 €  · 1 x 0,50 €  · 1 x 0,20 €  · 1 x 0,10 € ")]

    public void Desglose_ImporteValido_DevuelveMonedas(int centimos, string esperado)
    {
        string resultado = Program.Desglose(centimos);

        resultado = resultado
            .Replace('\u00A0', ' ')
            .Replace('\u202F', ' ');
        
        Assert.Equal(esperado, resultado);
    }

    [Xunit.Theory]
    [InlineData(5, -1, 8)]
    [InlineData(8, 1, 8)]
    [InlineData(5, int.MaxValue, 8)]
    public void NuevoStock_ValoresInvalidos_SaltaExcepcion(
        int actual, int cantidad, int maximo)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Program.NuevoStock(actual, cantidad, maximo));
    }
}