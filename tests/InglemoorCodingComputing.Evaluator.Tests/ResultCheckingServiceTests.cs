namespace InglemoorCodingComputing.Evaluator.UnitTests;

public class ResultCheckingServiceTests
{
    [Fact]
    public void TrueIfTheSame()
    {
        ResultCheckingService resultCheckingService = new();
        var x = "dfgs;hjksadfkl; jgbfdpiohgujsdfiogjhdpsioghypdfg iojaldfugihsdfpoigjdsfgil;dasfjgsoipdf;gjsdf;iogds gdsfgb jf[0923u409[ jgfdioj;bcvxb  fs O}_)S(() {^UHJFSDKLJ hgfdo8uig";
        Assert.True(resultCheckingService.Verify(x, x));
    }

    [Fact]
    public void FalseIfNotTheSame()
    {
        ResultCheckingService resultCheckingService = new();
        var x = "dfgs;hjksadfkl; jgbfdpiohgujsdfiogjhdpsioghypdfg iojaldfugihsdfpoigjdsfgil;dasfjgsoipdf;gjsdf;iogds gdsfgb jf[0923u409[ jgfdioj;bcvxb  fs O}_)S(() {^UHJFSDKLJ hgfdo8uig";
        var y = "dfgs;hjksadfkl; jgbfdpiohgujsdfiogjhdpsioghypdfg asdfdsfasdffiojaldfugihsdfpoigjdsfgil;dasfjgsoipdf;gjsdf;iogds gdsfgb jf[0923u409[ jgfdioj;bcvxb  fs O}_)S(() {^UHJFSDKLJ hgfdo8uig";
        Assert.False(resultCheckingService.Verify(x, y));
    }

    [Fact]
    public void ExtraNewLinesTrimmedOff()
    {
        ResultCheckingService resultCheckingService = new();
        var x = "\n\n\nasdf\n\n";
        var y = "asdf";
        Assert.True(resultCheckingService.Verify(x, y));
    }

    [Fact]
    public void InternalNewLinesNotTrimmedOff()
    {
        ResultCheckingService resultCheckingService = new();
        var x = "asdf\nasdf";
        var y = "asdfasdf";
        Assert.False(resultCheckingService.Verify(x, y));
    }
}
