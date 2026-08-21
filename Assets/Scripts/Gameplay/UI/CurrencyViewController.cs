
[UIDocument("CurrencyView")]
public class CurrencyViewController : UIViewController
{
    public UIImage currencyImage;
    public UILabel amountText;

    Currency currency;
    int amount;

    public override void Init()
    {
        currencyImage = view.Find<UIImage>(nameof(currencyImage));
        amountText = view.Find<UILabel>(nameof(amountText));
    }

    public void Setup(Currency currency, int amount)
    {
        this.currency = currency;
        this.amount = amount;
        Refresh();
    }

    public void Refresh()
    {
        currencyImage.sprite = currency.icon;
        amountText.text = amount.ToString();
    }

    public void RefreshAmount(int amount)
    {
        amountText.text = amount.ToString();
    }
}
