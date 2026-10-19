package ge.pompa.mobile;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;

import androidx.core.content.ContextCompat;

import com.getcapacitor.JSObject;
import com.getcapacitor.Plugin;
import com.getcapacitor.PluginCall;
import com.getcapacitor.PluginMethod;
import com.getcapacitor.annotation.CapacitorPlugin;
import com.google.android.gms.auth.api.phone.SmsRetriever;
import com.google.android.gms.common.api.CommonStatusCodes;
import com.google.android.gms.common.api.Status;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

@CapacitorPlugin(name = "SmsAutofill", requestCodes = { SmsAutofillPlugin.SMS_CONSENT_REQUEST })
public class SmsAutofillPlugin extends Plugin {
    static final int SMS_CONSENT_REQUEST = 4817;
    private static final Pattern OTP_PATTERN = Pattern.compile("(?<!\\d)\\d{4}(?!\\d)");

    private BroadcastReceiver smsReceiver;
    private PluginCall activeCall;

    @PluginMethod
    public void startListening(PluginCall call) {
        stopListening();
        activeCall = call;

        SmsRetriever.getClient(getContext())
            .startSmsUserConsent(null)
            .addOnSuccessListener(ignored -> {
                registerSmsReceiver();
                call.resolve();
            })
            .addOnFailureListener(error -> {
                activeCall = null;
                call.reject("SMS autofill could not be started", error);
            });
    }

    private void registerSmsReceiver() {
        smsReceiver = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                if (!SmsRetriever.SMS_RETRIEVED_ACTION.equals(intent.getAction()) || intent.getExtras() == null) {
                    return;
                }

                Status status = (Status) intent.getExtras().get(SmsRetriever.EXTRA_STATUS);
                if (status == null) return;

                if (status.getStatusCode() == CommonStatusCodes.SUCCESS) {
                    Intent consentIntent = intent.getParcelableExtra(SmsRetriever.EXTRA_CONSENT_INTENT);
                    if (consentIntent != null && activeCall != null) {
                        startActivityForResult(activeCall, consentIntent, SMS_CONSENT_REQUEST);
                    }
                } else if (status.getStatusCode() == CommonStatusCodes.TIMEOUT) {
                    stopListening();
                }
            }
        };

        IntentFilter filter = new IntentFilter(SmsRetriever.SMS_RETRIEVED_ACTION);
        ContextCompat.registerReceiver(
            getContext(),
            smsReceiver,
            filter,
            SmsRetriever.SEND_PERMISSION,
            null,
            ContextCompat.RECEIVER_EXPORTED
        );
    }

    @Override
    @SuppressWarnings("deprecation")
    protected void handleOnActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode != SMS_CONSENT_REQUEST) return;

        if (resultCode == Activity.RESULT_OK && data != null) {
            String message = data.getStringExtra(SmsRetriever.EXTRA_SMS_MESSAGE);
            Matcher matcher = OTP_PATTERN.matcher(message == null ? "" : message);
            if (matcher.find()) {
                JSObject result = new JSObject();
                result.put("code", matcher.group());
                notifyListeners("codeReceived", result, true);
            }
        }

        stopListening();
    }

    private void stopListening() {
        if (smsReceiver != null) {
            try {
                getContext().unregisterReceiver(smsReceiver);
            } catch (IllegalArgumentException ignored) {
                // The receiver may already have been removed by the system.
            }
            smsReceiver = null;
        }
        activeCall = null;
    }

    @Override
    protected void handleOnDestroy() {
        stopListening();
    }
}
