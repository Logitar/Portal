<template>
  <b-modal :id="id" :title="$t('user.session.signOut')">
    <p>
      {{ $t('user.session.confirm') }}
      <br />
      <span class="text-danger">{{ $d(new Date(session.updatedOn), 'medium') }}</span>
    </p>
    <slot />
    <template #modal-footer="{ cancel, ok }">
      <icon-button icon="ban" text="actions.cancel" @click="cancel()" />
      <icon-button :disabled="loading" icon="sign-out-alt" :loading="loading" text="user.session.signOut" variant="danger" @click="$emit('ok', ok)" />
    </template>
  </b-modal>
</template>

<script>
export default {
  name: 'SignOutModal',
  props: {
    id: {
      type: String,
      default: 'signOut'
    },
    loading: {
      type: Boolean,
      default: false
    },
    session: {
      type: Object,
      required: true
    }
  }
}
</script>
